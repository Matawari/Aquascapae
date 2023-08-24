using UnityEngine;
using System.Linq;

public class FishAIBehavior : MonoBehaviour
{
    public BoxCollider waterBodyCollider;
    public Transform[] hidingSpots;
    public GameObject foodSource;

    private bool isPredator = false;
    private float hungerTimer = 30.0f;
    private float currentHungerTimer;
    private float detectionRadius = 5.0f;
    private float fleeSpeed = 5.0f;
    private float chaseSpeed = 3.0f;
    private float wanderSpeed = 1.0f;
    private float rotationSpeed = 2.0f;
    private float avoidanceDistance = 2.0f;
    private float avoidanceForce = 2.0f;
    private float[] previousTurningSpeeds = new float[5];
    private int currentIndex = 0;

    private Vector3 currentVelocity;
    private float acceleration = 2.0f;
    private float deceleration = 4.0f;

    private float spiralAngle = 0f;
    private const float goldenRatio = 1.61803398875f;

    private float idleProbability = 0.01f;
    private float idleDuration = 5.0f;
    private float currentIdleTimer = 0.0f;

    private float surfaceProbability = 0.005f;
    private float surfaceDuration = 4.0f;
    private float currentSurfaceTimer = 0.0f;

    private BehaviorTreeNode rootBehavior;

    private void Start()
    {
        currentHungerTimer = hungerTimer;
        rootBehavior = new SelectorNode(
            new SequenceNode(
                new ConditionNode(IsIdle),
                new ActionNode(Idle)
            ),
            new SequenceNode(
                new ConditionNode(IsSurfacing),
                new ActionNode(Surface)
            ),
            new SequenceNode(
                new ConditionNode(IsHungry),
                new ActionNode(SeekFood)
            ),
            new SequenceNode(
                new ConditionNode(IsThreatened),
                new SelectorNode(
                    new SequenceNode(
                        new ConditionNode(IsPredator),
                        new ActionNode(Flee)
                    ),
                    new SequenceNode(
                        new ConditionNode(IsPrey),
                        new ActionNode(Chase)
                    )
                )
            ),
            new ActionNode(Wander)
        );
    }

    private void Update()
    {
        float averagePreviousTurningSpeed = previousTurningSpeeds.Average();
        float currentTurningSpeed = (averagePreviousTurningSpeed + RandomTurningSpeed()) / 2;
        previousTurningSpeeds[currentIndex] = currentTurningSpeed;
        currentIndex = (currentIndex + 1) % previousTurningSpeeds.Length;

        if (currentIdleTimer > 0)
        {
            currentIdleTimer -= Time.deltaTime;
            PivotTurn();
        }
        else if (currentSurfaceTimer > 0)
        {
            currentSurfaceTimer -= Time.deltaTime;
            rootBehavior.Execute();
        }
        else
        {
            if (IsObstacleAhead())
            {
                AvoidObstacle();
            }
            else
            {
                rootBehavior.Execute();
            }
        }

        float persistence = 0.5f;
        float theta = currentTurningSpeed * Time.deltaTime;
        Quaternion rotation = Quaternion.Euler(0f, theta, 0f);
    }

    private float RandomTurningSpeed()
    {
        return Random.Range(-1f, 1f) * wanderSpeed;
    }

    private bool IsObstacleAhead()
    {
        Vector3 forward = transform.forward;
        float rayLength = 2.0f;
        RaycastHit hit;

        if (Physics.Raycast(transform.position, forward, out hit, rayLength))
        {
            return true;
        }

        return false;
    }

    private void PivotTurn()
    {
        float pivotSpeed = 30.0f;
        transform.Rotate(Vector3.up, pivotSpeed * Time.deltaTime);
    }

    private void AvoidObstacle()
    {
        Vector3 intendedPosition = transform.position + currentVelocity * Time.deltaTime;
        Vector3 avoidDirection = CalculateAvoidanceDirection(intendedPosition);

        if (IsNearWall())
        {
            float wallInfluence = 2.0f;
            avoidDirection *= wallInfluence;
        }

        ApplyAvoidance(avoidDirection);
    }

    private void ApplyAvoidance(Vector3 avoidDirection)
    {
        float avoidSpeed = 1.0f;

        Vector3 adjustedDirection = AvoidanceRaycast(avoidDirection);
        ApplyAcceleration(adjustedDirection, avoidSpeed);
        ApplyVelocity();
    }

    private bool IsNearWall()
    {
        Bounds bounds = waterBodyCollider.bounds;
        float distanceToWall = Mathf.Min(
            transform.position.x - bounds.min.x,
            bounds.max.x - transform.position.x,
            transform.position.y - bounds.min.y,
            bounds.max.y - transform.position.y,
            transform.position.z - bounds.min.z,
            bounds.max.z - transform.position.z
        );

        return distanceToWall < 1.0f;
    }

    private Vector3 AvoidanceRaycast(Vector3 direction)
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, direction, out hit))
        {
            Vector3 newDirection = Vector3.Reflect(direction, hit.normal);
            return newDirection.normalized;
        }

        return direction;
    }

    private bool IsIdle()
    {
        if (currentIdleTimer > 0)
            return true;

        if (Random.value < idleProbability)
        {
            currentIdleTimer = idleDuration;
            return true;
        }

        return false;
    }

    private void Idle()
    {
        float idleSpeed = 0.1f; // Adjust speed as needed for slower movement
        float sinkRaiseAmount = Random.Range(-0.05f, 0.05f); // Adjust range for slower movement
        float headTiltAngle = sinkRaiseAmount * 15f; // Adjust angle for head tilt

        // Calculate the target position with slight sinking or raising
        Vector3 targetPosition = new Vector3(
            transform.position.x,
            transform.position.y + sinkRaiseAmount,
            transform.position.z
        );

        // Calculate the target rotation with head tilt
        Quaternion targetRotation = Quaternion.Euler(headTiltAngle, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);

        // Move towards the target position with idle speed
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, idleSpeed * Time.deltaTime);

        // Apply the target rotation with head tilt
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, idleSpeed * Time.deltaTime);
    }




    private bool IsSurfacing()
    {
        if (currentSurfaceTimer > 0)
            return true;

        if (Random.value < surfaceProbability)
        {
            currentSurfaceTimer = surfaceDuration;
            return true;
        }

        return false;
    }

    private void Surface()
    {
        float surfaceSpeed = 1.0f; // Adjust speed as needed

        // Calculate the direction towards the upper side of the box collider
        Vector3 surfaceDirection = Vector3.up;

        // Calculate the target position at the upper side of the box collider
        Vector3 targetPosition = new Vector3(
            transform.position.x,
            waterBodyCollider.bounds.max.y,
            transform.position.z
        );

        // Move towards the target position
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, surfaceSpeed * Time.deltaTime);
    }

    private bool IsHungry()
    {
        return currentHungerTimer <= 0.0f;
    }

    private bool IsThreatened()
    {
        Collider[] nearbyObjects = Physics.OverlapSphere(transform.position, detectionRadius);
        foreach (var obj in nearbyObjects)
        {
            FishAIBehavior otherFish = obj.GetComponent<FishAIBehavior>();
            if (otherFish && otherFish.isPredator != isPredator)
            {
                return true;
            }
        }
        return false;
    }

    private bool IsPredator()
    {
        return isPredator;
    }

    private bool IsPrey()
    {
        return !isPredator;
    }

    private void SeekFood()
    {
        if (foodSource != null)
        {
            Vector3 foodDirection = (foodSource.transform.position - transform.position).normalized;
            transform.Translate(foodDirection * Time.deltaTime);
        }
    }

    private void ApplyAcceleration(Vector3 targetDirection, float speed)
    {
        currentVelocity = Vector3.MoveTowards(currentVelocity, targetDirection * speed, acceleration * Time.deltaTime);
    }

    private void ApplyDeceleration()
    {
        currentVelocity = Vector3.MoveTowards(currentVelocity, Vector3.zero, deceleration * Time.deltaTime);
    }

    private void ApplyVelocity()
    {
        transform.Translate(currentVelocity * Time.deltaTime, Space.World);
    }

    private void Flee()
    {
        FishAIBehavior nearestPredator = FindNearestPredator();

        if (nearestPredator != null)
        {
            Vector3 fleeDirection = transform.position - nearestPredator.transform.position;
            fleeDirection.Normalize();
            Vector3 adjustedDirection = AdjustForBounds(fleeDirection * fleeSpeed * Time.deltaTime);
            AdjustDirection(adjustedDirection);
            transform.Translate(Vector3.forward * fleeSpeed * Time.deltaTime);

            ApplyAcceleration(adjustedDirection, fleeSpeed);
            ApplyVelocity();
        }
    }

    private void Chase()
    {
        FishAIBehavior targetFish = FindTargetFish();

        if (targetFish != null)
        {
            Vector3 chaseDirection = targetFish.transform.position - transform.position;
            chaseDirection.Normalize();
            Vector3 adjustedDirection = AdjustForBounds(chaseDirection * chaseSpeed * Time.deltaTime);
            AdjustDirection(adjustedDirection);
            transform.Translate(Vector3.forward * chaseSpeed * Time.deltaTime);

            ApplyAcceleration(adjustedDirection, chaseSpeed);
            ApplyVelocity();
        }
    }

    private void Wander()
    {
        // Incorporate characteristics of persistent turning walker model here
        float persistence = 0.5f; // Adjust persistence parameter
        float meanTurningRate = 0.0f; // Adjust mean turning rate

        float deltaTime = Time.deltaTime;
        float theta = Random.Range(-1f, 1f) * meanTurningRate * deltaTime;
        Quaternion rotation = Quaternion.Euler(0f, theta, 0f);

        Vector3 forward = transform.forward;
        Vector3 rotatedForward = rotation * forward;

        // Calculate new position based on rotated direction
        Vector3 newPosition = transform.position + rotatedForward * wanderSpeed * deltaTime;

        // Apply avoidance logic to avoid obstacles or collisions
        newPosition = AvoidObstacles(newPosition);

        // Apply bounds checking to keep fish within the water body
        newPosition = AdjustForBounds(newPosition);

        // Update position and rotation
        transform.position = newPosition;
        transform.rotation = Quaternion.LookRotation(rotatedForward);

        // Apply acceleration and velocity
        ApplyAcceleration(rotatedForward, wanderSpeed);
        ApplyVelocity();
    }

    private Vector3 AvoidObstacles(Vector3 intendedPosition)
    {
        Vector3 avoidDirection = CalculateAvoidanceDirection(intendedPosition);
        RaycastHit hit;

        // Cast a ray to check if there's an obstacle in the avoidDirection
        if (Physics.Raycast(transform.position, avoidDirection, out hit, avoidanceDistance))
        {
            Vector3 newDirection = Vector3.Reflect(avoidDirection, hit.normal);
            return intendedPosition + newDirection * avoidanceForce * Time.deltaTime;
        }

        return intendedPosition;
    }

    private Vector3 CalculateAvoidanceDirection(Vector3 intendedPosition)
    {
        // Calculate direction away from obstacles or collisions
        Vector3 avoidDirection = intendedPosition - transform.position;
        avoidDirection.Normalize();
        return avoidDirection;
    }
    private Vector3 AdjustForBounds(Vector3 intendedDirection)
    {
        Bounds bounds = waterBodyCollider.bounds;
        Vector3 nextPosition = transform.position + intendedDirection;

        if (!bounds.Contains(nextPosition))
        {
            Vector3 toCenter = (bounds.center - transform.position).normalized;
            intendedDirection = Vector3.Lerp(intendedDirection, toCenter, 0.5f);
        }

        return intendedDirection;
    }

    private void AdjustDirection(Vector3 newDirection)
    {
        Quaternion targetRotation = Quaternion.LookRotation(newDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    private FishAIBehavior FindNearestPredator()
    {
        FishAIBehavior nearestPredator = null;
        float nearestDistance = float.MaxValue;
        Collider[] nearbyObjects = Physics.OverlapSphere(transform.position, detectionRadius);
        foreach (var obj in nearbyObjects)
        {
            FishAIBehavior otherFish = obj.GetComponent<FishAIBehavior>();
            if (otherFish && otherFish.isPredator)
            {
                float distance = Vector3.Distance(transform.position, otherFish.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestPredator = otherFish;
                }
            }
        }
        return nearestPredator;
    }

    private FishAIBehavior FindTargetFish()
    {
        FishAIBehavior targetFish = null;
        float nearestDistance = float.MaxValue;
        Collider[] nearbyObjects = Physics.OverlapSphere(transform.position, detectionRadius);
        foreach (var obj in nearbyObjects)
        {
            FishAIBehavior otherFish = obj.GetComponent<FishAIBehavior>();
            if (otherFish && !otherFish.isPredator && otherFish != this)
            {
                float distance = Vector3.Distance(transform.position, otherFish.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    targetFish = otherFish;
                }
            }
        }
        return targetFish;
    }
}
