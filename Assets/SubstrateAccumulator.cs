using UnityEngine;

public class SubstrateAccumulator : MonoBehaviour
{
    public int gridSize = 10;  // Number of cells in one dimension
    public float cellSize = 0.5f;  // Size of each cell
    public MeshFilter tankBottomMeshFilter;
    private Mesh tankBottomMesh;


    private float[,] substrateHeight;

    private void Start()
    {
        substrateHeight = new float[gridSize, gridSize];
        tankBottomMesh = tankBottomMeshFilter.mesh;

    }

    public void Accumulate(Vector3 position)
    {
        int x = Mathf.FloorToInt(position.x / cellSize);
        int z = Mathf.FloorToInt(position.z / cellSize);

        if (x >= 0 && x < gridSize && z >= 0 && z < gridSize)
        {
            substrateHeight[x, z] += 0.01f;  // Increment based on how much substrate to add

            // Deform the mesh
            Vector3[] vertices = tankBottomMesh.vertices;
            for (int i = 0; i < vertices.Length; i++)
            {
                // Convert the global vertex position to local mesh coordinates
                Vector3 localPos = tankBottomMeshFilter.transform.InverseTransformPoint(vertices[i]);

                int vertexX = Mathf.FloorToInt(localPos.x / cellSize);
                int vertexZ = Mathf.FloorToInt(localPos.z / cellSize);

                if (vertexX == x && vertexZ == z)
                {
                    vertices[i].y += 0.01f;  // Adjust this value for more/less deformation
                }
            }
            tankBottomMesh.vertices = vertices;
            tankBottomMesh.RecalculateNormals();  // Ensure lighting remains consistent
        }
    }

}
