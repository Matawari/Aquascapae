using UnityEngine;

public class WaterSimulation : MonoBehaviour
{
    public float waveAmplitude = 0.1f;
    public float waveRadius = 1.0f;
    public float waveDuration = 1.0f;

    public Camera mainCamera; // Reference to the camera

    private Vector3[] baseVertices;
    private MeshFilter meshFilter;

    private void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        baseVertices = meshFilter.mesh.vertices;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 clickPosition = GetMouseClickPosition();
            CreateCircularWave(clickPosition);
        }
    }

    private Vector3 GetMouseClickPosition()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition); // Use the specified camera
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            return hit.point;
        }

        return Vector3.zero;
    }

    private void CreateCircularWave(Vector3 center)
    {
        Mesh mesh = meshFilter.mesh;
        Vector3[] vertices = new Vector3[baseVertices.Length];

        for (int i = 0; i < baseVertices.Length; i++)
        {
            Vector3 vertex = baseVertices[i];
            float distanceToCenter = Vector3.Distance(vertex, center);

            if (distanceToCenter <= waveRadius)
            {
                float waveHeight = waveAmplitude * Mathf.Sin(Mathf.PI * distanceToCenter / waveRadius);
                vertex.y += waveHeight;
            }

            vertices[i] = vertex;
        }

        mesh.vertices = vertices;
        mesh.RecalculateNormals();
    }
}
