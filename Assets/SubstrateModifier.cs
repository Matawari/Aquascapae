using UnityEngine;

public class SubstrateModifier : MonoBehaviour
{
    public float modificationStrength = 0.1f;
    public float radius = 0.1f;
    public ParticleSystem particleSystem; // Reference to your particle system

    private void Start()
    {
        // Assign the OnParticleCollision function to the particle system's collision events
        ParticleSystem.CollisionModule collisionModule = particleSystem.collision;
        collisionModule.enabled = true;
        collisionModule.SetPlane(0, transform);
    }

    private void OnParticleCollision(GameObject other)
    {
        // Check if the game is not paused or fast-forwarded
        if (!TimeController.IsGamePausedOrFastForwarded())
        {
            ModifySubstrate(other.transform.position);
        }
    }

    private void ModifySubstrate(Vector3 collisionPoint)
    {
        // Ensure that the game is not paused or fast-forwarded before making modifications
        if (!TimeController.IsGamePausedOrFastForwarded())
        {
            RaycastHit hit;

            if (Physics.Raycast(collisionPoint, Vector3.down, out hit, Mathf.Infinity, LayerMask.GetMask("Substrate")))
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
