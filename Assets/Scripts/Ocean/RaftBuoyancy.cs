
using UnityEngine;

namespace Ocean
{
    public class RaftBuoyancy : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private OceanController oceanController;
        
        [SerializeField] private Transform oceanSurface;

        [Header("Raft Dimensions")]
        [Min(0.1f)]
        [SerializeField] private float sampleDistance = 2f;

        [Header("Movement Intensity")]
        [Range(0f, 1f)]
        [SerializeField] private float heaveStrength = 0.4f;

        [Range(0f, 1f)]
        [SerializeField] private float tiltStrength = 0.35f;

        [Header("Comfort Limits")]
        [Min(0f)]
        [SerializeField] private float maxHeave = 0.12f;

        [Range(0f, 10f)]
        [SerializeField] private float maxRoll = 1.5f;

        [Range(0f, 10f)]
        [SerializeField] private float maxPitch = 1f;

        [Header("Smoothing")]
        [Min(0.1f)]
        [SerializeField] private float responsiveness = 2.5f;

        private Vector3 initialPosition;
        private Quaternion initialRotation;

        private void Awake()
        {
            initialPosition = transform.localPosition;
            initialRotation = transform.localRotation;
        }

        private void Start()
        {
            if (oceanController == null)
                oceanController = FindAnyObjectByType<OceanController>();

            if (oceanController == null)
            {
                Debug.LogError(
                    "[RaftBuoyancy] OceanController not found.",
                    this
                );

                enabled = false;
            }
        }

        private void LateUpdate()
        {
            if (oceanController == null)
                return;

            Vector3 center = transform.parent != null
                ? transform.parent.TransformPoint(initialPosition)
                : initialPosition;

            float distance = sampleDistance;

            float centerHeight = SampleHeight(center);

            float leftHeight = SampleHeight(
                center + Vector3.left * distance);

            float rightHeight = SampleHeight(
                center + Vector3.right * distance);

            float frontHeight = SampleHeight(
                center + Vector3.forward * distance);

            float backHeight = SampleHeight(
                center + Vector3.back * distance);

            float heave = Mathf.Clamp(
                centerHeight * heaveStrength,
                -maxHeave,
                maxHeave
            );

            float roll = Mathf.Atan2(
                leftHeight - rightHeight,
                2f * distance
            ) * Mathf.Rad2Deg * tiltStrength;

            float pitch = Mathf.Atan2(
                frontHeight - backHeight,
                2f * distance
            ) * Mathf.Rad2Deg * tiltStrength;

            roll = Mathf.Clamp(roll, -maxRoll, maxRoll);
            pitch = Mathf.Clamp(pitch, -maxPitch, maxPitch);

            Vector3 targetPosition =
                initialPosition + Vector3.up * heave;

            Quaternion targetRotation =
                initialRotation *
                Quaternion.Euler(pitch, 0f, roll);

            float smoothing =
                1f - Mathf.Exp(-responsiveness * Time.deltaTime);

            transform.localPosition = Vector3.Lerp(
                transform.localPosition,
                targetPosition,
                smoothing
            );

            transform.localRotation = Quaternion.Slerp(
                transform.localRotation,
                targetRotation,
                smoothing
            );
        }

        private float SampleHeight(Vector3 position)
        {
            float waveHeight = oceanController.GetWaterHeight(position);

            if (oceanSurface == null)
                return waveHeight;

            float absoluteWaterHeight = oceanSurface.position.y + waveHeight;

            // A flutuação utiliza a oscilação relativa ao nível médio do mar.
            // A altura de repouso da jangada permanece independente.
            return absoluteWaterHeight - oceanSurface.position.y;
        }
    }
}
