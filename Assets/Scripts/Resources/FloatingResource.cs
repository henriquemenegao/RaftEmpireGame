using Ocean;
using UnityEngine;

namespace Resources
{
    public class FloatingResource : MonoBehaviour
    {
        [Header("Ocean References")]
        [SerializeField] private OceanController oceanController;
        [SerializeField] private Transform oceanSurface;

        [Header("Floating Settings")]
        [SerializeField] private float heightOffset = 0.05f;
        [SerializeField] private float smoothSpeed = 5f;

        [Header("Rotation Settings")]
        [SerializeField] private float rotationStrength = 3f;
        [SerializeField] private float rotationSpeed = 1f;

        private float rotationOffset;

        private void Start()
        {
            if (oceanController == null)
                oceanController = FindAnyObjectByType<OceanController>();

            if (oceanSurface == null && oceanController != null)
            {
                Transform surface = oceanController.transform.Find("OceanSurface");

                if (surface != null)
                    oceanSurface = surface;
            }

            rotationOffset = Random.Range(0f, 100f);
        }

        private void LateUpdate()
        {
            if (oceanController == null || oceanSurface == null)
                return;

            Vector3 position = transform.position;

            float waterHeight =
                oceanSurface.position.y +
                oceanController.GetWaterHeight(position);

            float targetY = waterHeight + heightOffset;

            position.y = targetY;

            transform.position = position;

            float rotationX = Mathf.Sin(
                Time.time * rotationSpeed + rotationOffset
            ) * rotationStrength;

            float rotationZ = Mathf.Cos(
                Time.time * rotationSpeed * 0.8f + rotationOffset
            ) * rotationStrength;

            transform.rotation = Quaternion.Euler(
                rotationX,
                0f,
                rotationZ
            );
        }
    }
}