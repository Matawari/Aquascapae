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
    public float minPouringAngle = 20f; // Minimum angle to start pouring
    public float maxPouringAngle = 90f; // Maximum angle to continue pouring

    private Vector3 targetPosition;
    private Quaternion targetRotation;
    private float initialElevation;
    private bool isPouring = false; // Flag to track whether the substrate is currently pouring or not

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
        Debug.Log("BagController Update is running");

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

        // Check the pouring angle
        if (transform.rotation.eulerAngles.z > minPouringAngle && transform.rotation.eulerAngles.z < maxPouringAngle)
        {
            if (!isPouring)
            {
                substrateEmitter.Play();
                isPouring = true;
            }
        }
        else
        {
            if (isPouring)
            {
                substrateEmitter.Stop();
                isPouring = false;
            }
        }
    }

    private void OnParticleCollision(GameObject other)
    {
        Debug.Log("Particle Collision Detected");

        if (other.CompareTag("Base"))
        {
            Debug.Log("Particle collided with Base");
            Vector3 collisionPoint = other.transform.position;
            substrateAccumulator.Accumulate(collisionPoint);
        }
    }
}
