using UnityEngine;

public class SubstrateModifier : MonoBehaviour
{
    public float modificationStrength = 0.1f;
    public float radius = 0.1f;

    private void Update()
    {
        // Check if the game is not paused or fast-forwarded
        if (!TimeController.IsGamePausedOrFastForwarded())
        {
            if (Input.GetMouseButton(0))
            {
                ModifySubstrate();
            }
        }
    }

    private void ModifySubstrate()
    {
        // Ensure that the game is not paused or fast-forwarded before making modifications
        if (!TimeController.IsGamePausedOrFastForwarded())
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Substrate")))
            {
                Mesh mesh = hit.collider.GetComponent<MeshFilter>().mesh;
                Vector3[] vertices = mesh.vertices;

                // Convert the global hit.point to local space
                Vector3 localPoint = hit.transform.InverseTransformPoint(hit.point);

                for (int i = 0; i < vertices.Length; i++)
                {
                    // Calculate the distance between the vertex and the local contact point
                    float distance = Vector3.Distance(vertices[i], localPoint);

                    // Apply modification only to the vertices within the specified radius
                    if (distance < radius)
                    {
                        // Calculate the falloff factor based on distance (1 at contact point, 0 at the radius)
                        float falloff = 1f - Mathf.Clamp01(distance / radius);

                        // Modify the X position of the vertex to move it up along the -X axis
                        vertices[i].x -= modificationStrength * falloff * Time.deltaTime;
                    }
                }

                mesh.vertices = vertices;
                mesh.RecalculateNormals();
            }
        }
    }
}
