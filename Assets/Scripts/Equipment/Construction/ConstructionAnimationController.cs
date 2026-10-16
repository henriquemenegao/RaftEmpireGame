using System;
using System.Collections;
using Equipment.Progression;
using UI.Construction;
using UI.Hud.Resources;
using UnityEngine;

namespace Equipment.Construction
{
    public class ConstructionAnimationController : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private ConstructionAnimationSettings settings;

        [Header("UI References")]
        [SerializeField] private ResourceBarUI resourceBar;
        [SerializeField] private Canvas effectsCanvas;
        [SerializeField] private RectTransform flyingResourcesContainer;
        [SerializeField] private FlyingResourceUI flyingResourcePrefab;

        [Header("World References")]
        [SerializeField] private Camera worldCamera;

        [Header("Equipment Animation")]
        [SerializeField] private EquipmentBuildAnimation equipmentBuildAnimation;

        public bool IsPlaying { get; private set; }

        public void PlayWithEquipment(
            Transform target,
            EquipmentDefinition definition,
            Transform equipment,
            Action onComplete = null)
        {
            if (!ValidateReferences(target, definition))
                return;

            if (equipment == null || equipmentBuildAnimation == null)
            {
                Debug.LogError(
                    "ConstructionAnimationController: equipamento ou animador ausente.");
                return;
            }

            IsPlaying = true;
            StartCoroutine(PlayWithEquipmentRoutine(
                target, definition, equipment, onComplete));
        }

        private bool ValidateReferences(
            Transform target,
            EquipmentDefinition definition)
        {
            if (IsPlaying)
            {
                Debug.LogWarning("Uma construção já está em andamento.");
                return false;
            }

            if (target == null || definition == null ||
                settings == null || resourceBar == null ||
                effectsCanvas == null ||
                flyingResourcesContainer == null ||
                flyingResourcePrefab == null)
            {
                Debug.LogError(
                    "ConstructionAnimationController: referências incompletas.");
                return false;
            }

            if (worldCamera == null)
                worldCamera = Camera.main;

            if (worldCamera == null)
            {
                Debug.LogError(
                    "ConstructionAnimationController: câmera não encontrada.");
                return false;
            }

            return true;
        }

        private IEnumerator PlayWithEquipmentRoutine(
            Transform target,
            EquipmentDefinition definition,
            Transform equipment,
            Action onComplete)
        {
            int resourceCount = CountFlyingResources(definition);

            if (resourceCount == 0)
            {
                Debug.LogWarning(
                    "Nenhum recurso visual disponível para animar a construção.");
                IsPlaying = false;
                onComplete?.Invoke();
                yield break;
            }

            bool buildCompleted = false;

            equipmentBuildAnimation.Begin(
                equipment,
                resourceCount,
                settings,
                () => buildCompleted = true);

            yield return new WaitForSeconds(settings.InitialDelay);

            yield return SpawnResourcesRoutine(
                target,
                definition,
                () => equipmentBuildAnimation.RegisterImpact());

            yield return new WaitUntil(() => buildCompleted || equipment == null);

            if (equipment == null)
            {
                Debug.LogWarning(
                    "ConstructionAnimationController: equipamento destruído durante a construção.");
                IsPlaying = false;
                yield break;
            }

            IsPlaying = false;
            onComplete?.Invoke();
        }

        private int CountFlyingResources(EquipmentDefinition definition)
        {
            int count = 0;
            var level = definition.GetLevel(1);

            if (level == null || level.Costs == null)
                return 0;

            foreach (var cost in level.Costs)
            {
                if (cost == null || cost.Amount <= 0)
                    continue;

                if (!resourceBar.TryGetResourceItem(
                    cost.Resource, out var item))
                    continue;

                if (item.Icon == null || item.IconRect == null)
                    continue;

                count += settings.ParticlesPerResource;
            }

            return count;
        }

        private IEnumerator SpawnResourcesRoutine(
            Transform target,
            EquipmentDefinition definition,
            Action onResourceArrived)
        {
            var level = definition.GetLevel(1);

            if (level == null || level.Costs == null)
                yield break;

            foreach (var cost in level.Costs)
            {
                if (cost == null || cost.Amount <= 0)
                    continue;

                if (!resourceBar.TryGetResourceItem(
                    cost.Resource, out var resourceItem))
                    continue;

                if (resourceItem.Icon == null || resourceItem.IconRect == null)
                    continue;

                for (int i = 0; i < settings.ParticlesPerResource; i++)
                {
                    SpawnFlyingResource(
                        resourceItem,
                        target,
                        onResourceArrived);

                    yield return new WaitForSeconds(settings.SpawnInterval);
                }
            }
        }

        private void SpawnFlyingResource(
            ResourceItemUI resourceItem,
            Transform target,
            Action onArrived)
        {
            Vector2 startScreenPosition =
                RectTransformUtility.WorldToScreenPoint(
                    GetCanvasCamera(resourceItem.IconRect),
                    resourceItem.IconRect.position);

            Vector2 startLocalPosition =
                ScreenToContainerPosition(startScreenPosition);

            var flyingResource = Instantiate(
                flyingResourcePrefab,
                flyingResourcesContainer);

            flyingResource.Play(
                resourceItem.Icon,
                startLocalPosition,
                () =>
                {
                    if (target == null)
                        return startLocalPosition;

                    Vector2 targetScreenPosition =
                        worldCamera.WorldToScreenPoint(target.position);

                    return ScreenToContainerPosition(targetScreenPosition);
                },
                settings.FlightDuration,
                settings.ArcHeight,
                settings.ResourceIconScale,
                onArrived);
        }

        private Vector2 ScreenToContainerPosition(Vector2 screenPosition)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                flyingResourcesContainer,
                screenPosition,
                effectsCanvas.renderMode == RenderMode.ScreenSpaceOverlay
                    ? null
                    : effectsCanvas.worldCamera,
                out Vector2 localPosition);

            return localPosition;
        }

        private Camera GetCanvasCamera(RectTransform element)
        {
            Canvas sourceCanvas = element.GetComponentInParent<Canvas>();

            if (sourceCanvas == null ||
                sourceCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
                return null;

            return sourceCanvas.worldCamera;
        }
    }
}
