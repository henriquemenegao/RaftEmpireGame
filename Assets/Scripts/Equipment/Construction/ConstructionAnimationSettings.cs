using UnityEngine;

namespace Equipment.Construction
{
    [CreateAssetMenu(
        fileName = "ConstructionAnimationSettings",
        menuName = "Raft Empire/Construction/Animation Settings")]
    public class ConstructionAnimationSettings : ScriptableObject
    {
        [Header("Resource Flight")]
        [Min(1)] public int ParticlesPerResource = 6;
        [Min(0f)] public float InitialDelay = 0.15f;
        [Min(0.01f)] public float FlightDuration = 0.8f;
        [Min(0f)] public float SpawnInterval = 0.06f;
        [Min(0f)] public float ArcHeight = 100f;
        [Min(0.1f)] public float ResourceIconScale = 1f;

        [Header("Equipment Growth")]
        [Min(0f)] public float BuildStartScale = 0f;
        [Min(0f)] public float BuildEndScale = 1f;
        [Min(0.01f)] public float GrowthSmoothness = 0.12f;

        [Header("Resource Impact")]
        [Min(0f)] public float ImpactBounceStrength = 0.08f;
        [Min(0.01f)] public float ImpactBounceDuration = 0.18f;

        [Header("Final Boing")]
        [Min(1f)] public float BoingOvershoot = 1.15f;
        [Min(0.01f)] public float BoingDuration = 0.3f;
    }
}