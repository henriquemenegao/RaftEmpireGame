
using System;
using System.Collections;
using UnityEngine;

namespace Equipment.Construction
{
    public class EquipmentBuildAnimation : MonoBehaviour
    {
        private Coroutine currentAnimation;

        private Transform equipment;
        private ConstructionAnimationSettings settings;

        private Vector3 originalScale;

        private int totalResources;
        private int arrivedResources;

        private float currentGrowth;
        private float growthVelocity;

        private float impactElapsed;
        private bool impactActive;
        private bool finishing;

        private Action onComplete;

        public void Begin(
            Transform targetEquipment,
            int resourceCount,
            ConstructionAnimationSettings animationSettings,
            Action completed = null)
        {
            if (targetEquipment == null || animationSettings == null)
            {
                Debug.LogWarning(
                    "EquipmentBuildAnimation: referências inválidas.");
                return;
            }

            if (currentAnimation != null)
                StopCoroutine(currentAnimation);

            equipment = targetEquipment;
            settings = animationSettings;

            originalScale = equipment.localScale;

            totalResources = Mathf.Max(1, resourceCount);
            arrivedResources = 0;

            currentGrowth = settings.BuildStartScale;
            growthVelocity = 0f;

            impactElapsed = 0f;
            impactActive = false;
            finishing = false;

            onComplete = completed;

            equipment.localScale =
                originalScale * settings.BuildStartScale;

            currentAnimation = StartCoroutine(Animate());
        }

        public void RegisterImpact()
        {
            if (equipment == null || finishing)
                return;

            arrivedResources = Mathf.Min(
                arrivedResources + 1,
                totalResources);

            impactElapsed = 0f;
            impactActive = true;
        }

        private IEnumerator Animate()
        {
            while (equipment != null)
            {
                float targetGrowth = Mathf.Lerp(
                    settings.BuildStartScale,
                    settings.BuildEndScale,
                    (float)arrivedResources / totalResources);

                currentGrowth = Mathf.SmoothDamp(
                    currentGrowth,
                    targetGrowth,
                    ref growthVelocity,
                    settings.GrowthSmoothness);

                float impactScale = 0f;

                if (impactActive)
                {
                    impactElapsed += Time.deltaTime;

                    float t = Mathf.Clamp01(
                        impactElapsed /
                        settings.ImpactBounceDuration);

                    // Pequena expansão e retorno suave.
                    impactScale =
                        Mathf.Sin(t * Mathf.PI) *
                        settings.ImpactBounceStrength;

                    if (t >= 1f)
                        impactActive = false;
                }

                equipment.localScale =
                    originalScale *
                    (currentGrowth + impactScale);

                if (arrivedResources >= totalResources &&
                    Mathf.Abs(currentGrowth -
                              settings.BuildEndScale) < 0.01f)
                {
                    finishing = true;
                    break;
                }

                yield return null;
            }

            if (equipment == null)
                yield break;

            // Garante que o último impulso termine
            // antes do boing final.
            while (impactActive)
            {
                impactElapsed += Time.deltaTime;

                float t = Mathf.Clamp01(
                    impactElapsed /
                    settings.ImpactBounceDuration);

                float impactScale =
                    Mathf.Sin(t * Mathf.PI) *
                    settings.ImpactBounceStrength;

                equipment.localScale =
                    originalScale *
                    (settings.BuildEndScale + impactScale);

                impactActive = t < 1f;

                yield return null;
            }

            float elapsed = 0f;

            while (elapsed < settings.BoingDuration)
            {
                elapsed += Time.deltaTime;

                float t = Mathf.Clamp01(
                    elapsed / settings.BoingDuration);

                float bounce =
                    Mathf.Sin(t * Mathf.PI) *
                    (settings.BoingOvershoot -
                     settings.BuildEndScale);

                equipment.localScale =
                    originalScale *
                    (settings.BuildEndScale + bounce);

                yield return null;
            }

            equipment.localScale =
                originalScale * settings.BuildEndScale;

            currentAnimation = null;
            onComplete?.Invoke();
        }
    }
}
