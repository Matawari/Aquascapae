using UnityEngine;

public class BagController : MonoBehaviour
{
    public Transform tankBounds; // Assign the tank object here
    public Camera mainCamera; // Assign the main camera here
    public float rotationSpeed = 60f; // Rotation speed in degrees per second
    public float minRotation = 60f; // Minimum rotation in degrees
    public float maxRotation = 120f; // Maximum rotation in degrees
    public float lerpSpeed = 5f; // Lerp speed for smoother movement and rotation
    public float elevationSpeed = 0.1f; // Elevation speed when scrolling the mouse
    public float maxElevationAboveInitial = 2f; // Maximum elevation above initial position
    public ParticleSystem substrateEmitter;  // Assign the particle system in the Unity Editor
    public SubstrateAccumulator substrateAccumulator;



    private Vector3 targetPosition;
    private Quaternion targetRotation;
    private float initialElevation;

    private void Start()
    {
        // Initialize target position and rotation
        targetPosition = transform.position;
        targetRotation = transform.rotation;

        // Store the initial elevation for later reference
        initialElevation = transform.position.y;
    }

    private void Update()
    {
        // Elevation control with middle mouse scroll
        float scrollDelta = Input.GetAxis("Mouse ScrollWheel");
        float newElevation = targetPosition.y - scrollDelta * elevationSpeed;
        newElevation = Mathf.Clamp(newElevation, initialElevation, initialElevation + maxElevationAboveInitial);
        targetPosition.y = newElevation;

        // Rotation control
        float rotationChange = Input.GetKey(KeyCode.W) ? -rotationSpeed : (Input.GetKey(KeyCode.S) ? rotationSpeed : 0f);
        float newZRotation = Mathf.Clamp(transform.rotation.eulerAngles.z + rotationChange * Time.deltaTime, minRotation, maxRotation);
        targetRotation = Quaternion.Euler(targetRotation.eulerAngles.x, targetRotation.eulerAngles.y, newZRotation);

        // Mouse movement
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            if (hit.collider.transform == tankBounds)
            {
                // Update the target position based on mouse movement within the tank's bounds
                targetPosition = new Vector3(hit.point.x, targetPosition.y, hit.point.z);
            }
        }

        // Smoothly move and rotate the bag towards target values
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * lerpSpeed);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        if (transform.rotation.eulerAngles.z > 20f && transform.rotation.eulerAngles.z < 90f)  // Ensure rotation is between 20 and 90 degrees
        {
            if (!substrateEmitter.isPlaying)
            {
                substrateEmitter.Play();
            }
        }
        else
        {
            if (substrateEmitter.isPlaying)
            {
                substrateEmitter.Stop();
            }
        }
    }

    private void OnParticleCollision(GameObject other)
    {
        if (other.CompareTag("Base"))  // Assuming your tank has a tag "Tank"
        {
            Vector3 collisionPoint = other.transform.position;  // For simplicity, we're using the tank's position. You could get more precise collision points if necessary.
            substrateAccumulator.Accumulate(collisionPoint);
        }
    }

}
