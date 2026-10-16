
using UnityEngine;

namespace Ocean
{
    public class WaterContactFoam : MonoBehaviour
    {
        public enum FoamShape
        {
            Rectangle,
            Ellipse
        }

        [Header("Ocean")]
        [SerializeField] private OceanController oceanController;
        [SerializeField] private Transform oceanSurface;

        [Header("Foam Shape")]
        [SerializeField] private FoamShape shape = FoamShape.Rectangle;

        [Min(0.1f)]
        [SerializeField] private float width = 5f;

        [Min(0.1f)]
        [SerializeField] private float length = 5f;

        [Header("Foam Appearance")]
        [Range(0f, 1f)]
        [SerializeField] private float intensity = 0.35f;

        [Min(0f)]
        [SerializeField] private float waterOffset = 0.025f;

        [Header("Contact Geometry")]
        [SerializeField] private Renderer targetRenderer;

        [SerializeField, HideInInspector]
        private Vector3 localFoamCenter;

        public float Width => width;
        public float Length => length;
        public float Intensity => intensity;
        public FoamShape Shape => shape;
        public Vector3 LocalFoamCenter => localFoamCenter;

        private void Start()
        {
            if (oceanController == null)
                oceanController = FindAnyObjectByType<OceanController>();

            if (oceanSurface == null && oceanController != null)
            {
                Transform surface =
                    oceanController.transform.Find("OceanSurface");

                if (surface != null)
                    oceanSurface = surface;
            }

            if (oceanController == null || oceanSurface == null)
            {
                Debug.LogError(
                    "[WaterContactFoam] Missing ocean references.",
                    this
                );

                enabled = false;
            }
        }

        public float GetWaterSurfaceHeight(Vector3 worldPosition)
        {
            if (oceanController == null || oceanSurface == null)
                return worldPosition.y;

            return oceanSurface.position.y
                + oceanController.GetWaterHeight(worldPosition)
                + waterOffset;
        }

        [ContextMenu("Measure Raft Bounds")]
        private void MeasureRaftBounds()
        {
            if (targetRenderer == null)
            {
                Debug.LogWarning(
                    "[WaterContactFoam] Target Renderer not assigned.",
                    this
                );
                return;
            }

            Bounds bounds = targetRenderer.bounds;

            Debug.Log(
                $"[WaterContactFoam] " +
                $"Width X: {bounds.size.x:F2} | " +
                $"Length Z: {bounds.size.z:F2} | " +
                $"Center: {bounds.center}",
                this
            );
        }

        [ContextMenu("Apply Renderer Bounds")]
        private void ApplyRendererBounds()
        {
            if (targetRenderer == null)
            {
                Debug.LogWarning(
                    "[WaterContactFoam] Target Renderer not assigned.",
                    this
                );
                return;
            }

            Bounds bounds = targetRenderer.bounds;

            width = bounds.size.x / transform.lossyScale.x;
            length = bounds.size.z / transform.lossyScale.z;

            localFoamCenter =
                transform.InverseTransformPoint(bounds.center);

            Debug.Log(
                $"[WaterContactFoam] " +
                $"Area: {width:F2} x {length:F2} | " +
                $"Local Center: {localFoamCenter}",
                this
            );
        }
    }
}
