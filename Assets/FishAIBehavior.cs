using UnityEngine;
using UnityEngine.AI;

public class FishAIBehavior : MonoBehaviour
{
    public Transform[] hidingSpots;
    public GameObject foodSource;

    public float speed = 5.0f;
    public float rotationSpeed = 4.0f;
    public float detectionRadius = 5.0f;
    public bool isPredator = false;
    
    public float hungerTimer = 30.0f;
    public float desiredMinDepth = 0f; // You can adjust the default value as needed
    public float desiredMaxDepth = 10f; // You can adjust the default value as needed

    private bool isHungry = false;
    private float currentHungerTimer;
    private NavMeshAgent navAgent;

    private void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
        navAgent.speed = speed;
        navAgent.angularSpeed = rotationSpeed * 100; // Convert to degrees per second

        Wander();
        currentHungerTimer = hungerTimer;
    }

    private void Update()
    {
        if (navAgent.hasPath && navAgent.remainingDistance < 0.5f)
        {
            Wander();
        }

        HandleHunger();

        Collider[] nearbyObjects = Physics.OverlapSphere(transform.position, detectionRadius);
        foreach (var obj in nearbyObjects)
        {
            FishAIBehavior otherFish = obj.GetComponent<FishAIBehavior>();
            if (otherFish)
            {
                if (isPredator && !otherFish.isPredator)
                {
                    Chase(obj.transform.position);
                }
                else if (!isPredator && otherFish.isPredator)
                {
                    FleeFrom(obj.transform.position);
                }
            }
        }

        // Constrain fish to a certain depth
        Vector3 constrainedPosition = transform.position;
        constrainedPosition.y = Mathf.Clamp(constrainedPosition.y, desiredMinDepth, desiredMaxDepth);
        transform.position = constrainedPosition;
    }

    void Wander()
    {
        if (!navAgent.isOnNavMesh)
        {
            return; // Exit the method if the agent is not on a NavMesh
        }

        Vector3 randomDirection = Random.insideUnitSphere * detectionRadius;
        randomDirection.y = Mathf.Clamp(randomDirection.y, desiredMinDepth, desiredMaxDepth); // Ensure the fish stays within the desired depth
        randomDirection += transform.position;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, detectionRadius, navAgent.areaMask))
        {
            Vector3 finalPosition = hit.position;
            navAgent.SetDestination(finalPosition);
        }
    }

    void Chase(Vector3 target)
    {
        navAgent.SetDestination(target);
    }

    void FleeFrom(Vector3 dangerSource)
    {
        Vector3 fleeDirection = (transform.position - dangerSource).normalized * detectionRadius;
        navAgent.SetDestination(transform.position + fleeDirection);
    }

    void HandleHunger()
    {
        if (isHungry)
        {
            SeekFood();
        }
        else
        {
            currentHungerTimer -= Time.deltaTime;
            if (currentHungerTimer <= 0)
            {
                isHungry = true;
            }
        }
    }

    void SeekFood()
    {
        if (foodSource && navAgent.isOnNavMesh)
        {
            navAgent.SetDestination(foodSource.transform.position);
        }
    }


    public void Eat()
    {
        isHungry = false;
        currentHungerTimer = hungerTimer;
    }
}
