
using UnityEngine;

namespace Ocean
{
    [RequireComponent(typeof(WaterContactFoam))]
    public class WaterFoamRenderer : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private WaterContactFoam contactFoam;
        [SerializeField] private Material foamMaterial;

        [Header("Foam Settings")]
        [Range(4, 64)]
        [SerializeField] private int foamCount = 24;

        [Min(0.01f)]
        [SerializeField] private float patchSize = 0.16f;

        [Min(0f)]
        [SerializeField] private float edgeOffset = 0.08f;

        private GameObject foamObject;
        private Mesh foamMesh;
        private MeshFilter meshFilter;
        private MeshRenderer meshRenderer;

        private Vector3[] vertices;
        private Vector3[] localPatchPositions;

        private void Start()
        {
            if (contactFoam == null)
                contactFoam = GetComponent<WaterContactFoam>();

            if (contactFoam == null || foamMaterial == null)
            {
                Debug.LogWarning(
                    "[WaterFoamRenderer] Missing references.",
                    this
                );

                enabled = false;
                return;
            }

            CreateFoamObject();
            GenerateFoam();
        }

        private void CreateFoamObject()
        {
            foamObject = new GameObject("GeneratedContactFoam");

            foamObject.transform.SetParent(transform, false);

            foamObject.transform.localPosition = Vector3.zero;
            foamObject.transform.localRotation = Quaternion.identity;
            foamObject.transform.localScale = Vector3.one;

            meshFilter = foamObject.AddComponent<MeshFilter>();
            meshRenderer = foamObject.AddComponent<MeshRenderer>();

            meshRenderer.sharedMaterial = foamMaterial;

            meshRenderer.shadowCastingMode =
                UnityEngine.Rendering.ShadowCastingMode.Off;

            meshRenderer.receiveShadows = false;

            foamMesh = new Mesh
            {
                name = "Water Contact Foam"
            };

            meshFilter.sharedMesh = foamMesh;
        }

        private void GenerateFoam()
        {
            vertices = new Vector3[foamCount * 4];
            localPatchPositions = new Vector3[foamCount];

            int[] triangles = new int[foamCount * 6];

            float halfWidth =
                contactFoam.Width * 0.5f + edgeOffset;

            float halfLength =
                contactFoam.Length * 0.5f + edgeOffset;

            Vector3 center = contactFoam.LocalFoamCenter;

            for (int i = 0; i < foamCount; i++)
            {
                float t = (float)i / foamCount;

                float x;
                float z;

                if (contactFoam.Shape ==
                    WaterContactFoam.FoamShape.Ellipse)
                {
                    float angle = t * Mathf.PI * 2f;

                    x = Mathf.Cos(angle) * halfWidth;
                    z = Mathf.Sin(angle) * halfLength;
                }
                else
                {
                    float perimeterPosition = t * 4f;

                    if (perimeterPosition < 1f)
                    {
                        x = Mathf.Lerp(
                            -halfWidth,
                            halfWidth,
                            perimeterPosition
                        );

                        z = halfLength;
                    }
                    else if (perimeterPosition < 2f)
                    {
                        x = halfWidth;

                        z = Mathf.Lerp(
                            halfLength,
                            -halfLength,
                            perimeterPosition - 1f
                        );
                    }
                    else if (perimeterPosition < 3f)
                    {
                        x = Mathf.Lerp(
                            halfWidth,
                            -halfWidth,
                            perimeterPosition - 2f
                        );

                        z = -halfLength;
                    }
                    else
                    {
                        x = -halfWidth;

                        z = Mathf.Lerp(
                            -halfLength,
                            halfLength,
                            perimeterPosition - 3f
                        );
                    }
                }

                localPatchPositions[i] = new Vector3(
                    center.x + x,
                    0f,
                    center.z + z
                );

                int vertexIndex = i * 4;
                int triangleIndex = i * 6;

                triangles[triangleIndex] = vertexIndex;
                triangles[triangleIndex + 1] = vertexIndex + 2;
                triangles[triangleIndex + 2] = vertexIndex + 1;

                triangles[triangleIndex + 3] = vertexIndex + 2;
                triangles[triangleIndex + 4] = vertexIndex + 3;
                triangles[triangleIndex + 5] = vertexIndex + 1;
            }

            foamMesh.vertices = vertices;
            foamMesh.triangles = triangles;

            UpdateFoam();
        }

        private void LateUpdate()
        {
            if (foamMesh == null || contactFoam == null)
                return;

            UpdateFoam();
        }

        private void UpdateFoam()
        {
            float halfSize = patchSize * 0.5f;

            for (int i = 0; i < foamCount; i++)
            {
                Vector3 worldCenter =
                    transform.TransformPoint(
                        localPatchPositions[i]
                    );

                worldCenter.y =
                    contactFoam.GetWaterSurfaceHeight(worldCenter);

                Vector3 localCenter =
                    foamObject.transform.InverseTransformPoint(
                        worldCenter
                    );

                Vector3 right =
                    foamObject.transform.InverseTransformVector(
                        Vector3.right * halfSize
                    );

                Vector3 forward =
                    foamObject.transform.InverseTransformVector(
                        Vector3.forward * halfSize
                    );

                int index = i * 4;

                vertices[index] =
                    localCenter - right - forward;

                vertices[index + 1] =
                    localCenter + right - forward;

                vertices[index + 2] =
                    localCenter - right + forward;

                vertices[index + 3] =
                    localCenter + right + forward;
            }

            foamMesh.vertices = vertices;
            foamMesh.RecalculateBounds();
        }

        private void OnDestroy()
        {
            if (foamMesh != null)
                Destroy(foamMesh);

            if (foamObject != null)
                Destroy(foamObject);
        }
    }
}
