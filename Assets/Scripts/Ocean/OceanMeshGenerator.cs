
using UnityEngine;

namespace Ocean
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class OceanMeshGenerator : MonoBehaviour
    {
        [Header("Ocean Dimensions")]
        [Min(1f)]
        [SerializeField] private float size = 200f;

        [Range(2, 250)]
        [SerializeField] private int subdivisions = 100;

        private Mesh generatedMesh;

        private void Awake()
        {
            GenerateMesh();
        }

        private void GenerateMesh()
        {
            int verticesPerSide = subdivisions + 1;

            Vector3[] vertices = new Vector3[verticesPerSide * verticesPerSide];
            Vector2[] uvs = new Vector2[vertices.Length];
            int[] triangles = new int[subdivisions * subdivisions * 6];

            float step = size / subdivisions;
            float halfSize = size * 0.5f;

            for (int z = 0; z <= subdivisions; z++)
            {
                for (int x = 0; x <= subdivisions; x++)
                {
                    int index = z * verticesPerSide + x;

                    vertices[index] = new Vector3(
                        x * step - halfSize,
                        0f,
                        z * step - halfSize
                    );

                    uvs[index] = new Vector2(
                        (float)x / subdivisions,
                        (float)z / subdivisions
                    );
                }
            }

            int triangleIndex = 0;

            for (int z = 0; z < subdivisions; z++)
            {
                for (int x = 0; x < subdivisions; x++)
                {
                    int bottomLeft = z * verticesPerSide + x;
                    int bottomRight = bottomLeft + 1;
                    int topLeft = bottomLeft + verticesPerSide;
                    int topRight = topLeft + 1;

                    triangles[triangleIndex++] = bottomLeft;
                    triangles[triangleIndex++] = topLeft;
                    triangles[triangleIndex++] = bottomRight;

                    triangles[triangleIndex++] = bottomRight;
                    triangles[triangleIndex++] = topLeft;
                    triangles[triangleIndex++] = topRight;
                }
            }

            generatedMesh = new Mesh
            {
                name = "Generated Ocean Mesh",
                indexFormat = UnityEngine.Rendering.IndexFormat.UInt32
            };

            generatedMesh.vertices = vertices;
            generatedMesh.uv = uvs;
            generatedMesh.triangles = triangles;

            generatedMesh.RecalculateNormals();
            generatedMesh.RecalculateBounds();

            GetComponent<MeshFilter>().mesh = generatedMesh;
        }

        private void OnDestroy()
        {
            if (generatedMesh != null)
                Destroy(generatedMesh);
        }
    }
}
