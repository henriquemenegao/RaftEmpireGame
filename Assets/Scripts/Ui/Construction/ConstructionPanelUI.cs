
using System.Collections.Generic;
using Equipment.Construction;
using Equipment.Progression;
using Equipment.Slots;
using TMPro;
using UnityEngine;

namespace UI.Construction
{
    public class ConstructionPanelUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private EquipmentInventory equipmentInventory;
        [SerializeField] private RaftLevelDefinition raftLevel;
        [SerializeField] private ConstructionEquipmentCardUI cardPrefab;
        [SerializeField] private Transform buildContent;

        [Header("Construction Services")]
        [SerializeField] private EquipmentPurchaseService purchaseService;
        [SerializeField] private EquipmentInstallationService installationService;
        [SerializeField] private ConstructionAnimationController animationController;

        [Header("Resource Icons")]
        [SerializeField]
        private List<ResourceIconEntry> resourceIcons = new();

        private EquipmentSlot selectedSlot;

        private readonly List<ConstructionEquipmentCardUI>
            spawnedCards = new();

        private Dictionary<Resources.ResourceType, Sprite> iconLookup;

        private bool isBuilding;

        private void Awake()
        {
            iconLookup =
                new Dictionary<Resources.ResourceType, Sprite>();

            foreach (var entry in resourceIcons)
            {
                if (entry == null)
                    continue;

                iconLookup[entry.Resource] = entry.Icon;
            }
        }

        public void Open(EquipmentSlot slot)
        {
            if (slot == null || isBuilding)
                return;

            selectedSlot = slot;
            gameObject.SetActive(true);

            RefreshBuildList();
        }

        public void Close()
        {
            if (isBuilding)
                return;

            selectedSlot = null;
            gameObject.SetActive(false);
        }

        public void RefreshBuildList()
        {
            foreach (var card in spawnedCards)
            {
                if (card != null)
                    Destroy(card.gameObject);
            }

            spawnedCards.Clear();

            if (selectedSlot == null ||
                equipmentInventory == null ||
                raftLevel == null ||
                cardPrefab == null ||
                buildContent == null ||
                purchaseService == null ||
                installationService == null ||
                animationController == null)
            {
                Debug.LogWarning(
                    "ConstructionPanelUI: referências incompletas.");
                return;
            }

            foreach (var option in selectedSlot.AllowedEquipments)
            {
                if (option == null || option.Equipment == null)
                    continue;

                var definition = option.Equipment;

                int owned = equipmentInventory.Count(
                    definition.EquipmentId);

                int max = raftLevel.GetMaxEquipmentInstances(
                    definition);

                var status = EquipmentBuildValidator.Validate(
                    definition,
                    raftLevel,
                    equipmentInventory);

                bool canAfford =
                    purchaseService.CanAfford(definition);

                var card = Instantiate(cardPrefab, buildContent);

                card.Configure(
                    definition,
                    owned,
                    max,
                    status,
                    canAfford,
                    iconLookup,
                    () => Build(definition));

                spawnedCards.Add(card);
            }
        }

        private void Build(EquipmentDefinition definition)
        {
            if (isBuilding || selectedSlot == null ||
                definition == null)
                return;

            if (animationController.IsPlaying)
                return;

            var status = EquipmentBuildValidator.Validate(
                definition,
                raftLevel,
                equipmentInventory);

            if (status != EquipmentBuildStatus.Available ||
                !selectedSlot.Allows(definition) ||
                !purchaseService.CanAfford(definition))
            {
                RefreshBuildList();
                return;
            }

            // Ainda não possui um slot registrado.
            var instance = new EquipmentInstance(
                definition.EquipmentId);

            if (!installationService.CanInstall(
                    instance, definition, selectedSlot))
            {
                Debug.LogWarning(
                    "Não foi possível validar a instalação.");
                return;
            }

            if (!purchaseService.TryPay(definition))
            {
                RefreshBuildList();
                return;
            }

            var purchasedInstance =
                equipmentInventory.Add(definition);

            if (purchasedInstance == null)
            {
                Debug.LogError(
                    "Falha ao registrar equipamento comprado.");
                return;
            }

            if (!installationService.TryInstall(
                    purchasedInstance,
                    definition,
                    selectedSlot,
                    out GameObject installedObject))
            {
                Debug.LogError(
                    "Falha ao instalar equipamento comprado.");
                return;
            }

            isBuilding = true;

            var slot = selectedSlot;

            gameObject.SetActive(false);

            installedObject.SetActive(true);

            // Impede produção durante a animação.
            var behaviours =
                installedObject.GetComponentsInChildren<
                    MonoBehaviour>(true);

            foreach (var behaviour in behaviours)
                behaviour.enabled = false;

            animationController.PlayWithEquipment(
                slot.transform,
                definition,
                installedObject.transform,
                () =>
                {
                    if (installedObject != null)
                    {
                        foreach (var behaviour in behaviours)
                        {
                            if (behaviour != null)
                                behaviour.enabled = true;
                        }
                    }

                    isBuilding = false;
                    selectedSlot = null;

                    Debug.Log(
                        $"Construção concluída: {definition.DisplayName}");
                });
        }
    }
}
