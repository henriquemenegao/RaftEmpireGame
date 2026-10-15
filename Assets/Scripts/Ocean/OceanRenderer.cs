using UnityEngine;

namespace Ocean
{
    [RequireComponent(typeof(MeshFilter))]
    public class OceanRenderer : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private OceanController oceanController;

        private Mesh mesh;
        private Vector3[] originalVertices;
        private Vector3[] displacedVertices;

        private void Start()
        {
            if (oceanController == null)
                oceanController =
                    GetComponentInParent<OceanController>();

            if (oceanController == null)
            {
                Debug.LogError(
                    "[OceanRenderer] OceanController not found.",
                    this
                );

                enabled = false;
                return;
            }

            mesh = GetComponent<MeshFilter>().mesh;

            originalVertices = mesh.vertices;
            displacedVertices = new Vector3[originalVertices.Length];
        }

        private void Update()
        {
            if (mesh == null)
                return;

            for (int i = 0; i < originalVertices.Length; i++)
            {
                Vector3 worldPosition =
                    transform.TransformPoint(originalVertices[i]);

                Vector3 displacement =
                    oceanController.GetDisplacement(worldPosition);

                displacedVertices[i] =
                    originalVertices[i] +
                    transform.InverseTransformVector(displacement);
            }

            mesh.vertices = displacedVertices;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
        }
    }
}