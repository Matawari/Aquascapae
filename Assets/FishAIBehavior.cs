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
    private float fleeSpeed = 4.0f;
    private float chaseSpeed = 3.0f;
    private float wanderSpeed = 1.0f;
    private float rotationSpeed = 1.5f;
    private Vector3 currentVelocity;
    private float acceleration = 2.0f;
    private float deceleration = 4.0f;
    private float idleProbability = 0.01f;
    private float idleDuration = 5.0f;
    private float currentIdleTimer = 0.0f;
    private float surfaceProbability = 0.005f;
    private float surfaceDuration = 4.0f;
    private float currentSurfaceTimer = 0.0f;
    private BehaviorTreeNode rootBehavior;
    private float timeNearBoundary = 0f;
    private float idleTimer = 0.0f;

    private void Start()
    {
        currentHungerTimer = hungerTimer;
        InitializeBehaviorTree();
    }

    private void InitializeBehaviorTree()
    {
        rootBehavior = new SelectorNode(
            new SequenceNode(
                new ConditionNode(IsObstacleAhead),
                new ActionNode(AvoidObstacle)
            ),
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
                    new ActionNode(() => {
                        if (AmIPredator())
                            Flee();
                        else
                            Chase();
                    })
                )
            ),
            new ActionNode(Wander)
        );
    }

    private void Update()
    {
        if (IsOutsideWaterBody())
        {
            ReturnToWaterBody();
        }
        else
        {
            rootBehavior.Execute();
            if (IsNearBoundary())
            {
                TurnAwayFromBoundary();
            }
        }
        ApplyVelocity();
        SteerInsideWaterBody();
        StayWithinBounds();
        currentVelocity += SinusoidalMovement(1.0f, 0.1f);
        // Ensure no upward bias
        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, waterBodyCollider.bounds.min.y, waterBodyCollider.bounds.max.y), transform.position.z);
        ReflectFromBoundary();
        if (Random.value < 0.1f) // 10% chance to idle
        {
            Idle();
        }
        else
        {
            Wander();
        }
        if (IsNearBoundary())
        {
            TurnAwayFromBoundary();
            Idle();
        }
        else
        {
            if (idleTimer <= 0)
            {
                Wander(); // You can reintroduce the Wander behavior here for when they are not idling
            }
            else
            {
                Idle();
            }
        }

        ApplyVelocity();
    }

    private void ReflectFromBoundary()
    {
        Bounds bounds = waterBodyCollider.bounds;

        if (transform.position.x <= bounds.min.x || transform.position.x >= bounds.max.x)
        {
            currentVelocity.x = -currentVelocity.x;
            transform.position = new Vector3(Mathf.Clamp(transform.position.x, bounds.min.x, bounds.max.x), transform.position.y, transform.position.z);
        }

        if (transform.position.y <= bounds.min.y || transform.position.y >= bounds.max.y)
        {
            currentVelocity.y = -currentVelocity.y;
            transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, bounds.min.y, bounds.max.y), transform.position.z);
        }

        if (transform.position.z <= bounds.min.z || transform.position.z >= bounds.max.z)
        {
            currentVelocity.z = -currentVelocity.z;
            transform.position = new Vector3(transform.position.x, transform.position.y, Mathf.Clamp(transform.position.z, bounds.min.z, bounds.max.z));
        }
    }



    private Vector3 SinusoidalMovement(float frequency, float amplitude)
    {
        float sinWave = Mathf.Sin(Time.time * frequency) * amplitude;
        return new Vector3(0, sinWave, 0);
    }

    private void StayWithinBounds()
    {
        Bounds bounds = waterBodyCollider.bounds;
        Vector3 desiredDirection = transform.forward;

        if (!bounds.Contains(transform.position))
        {
            Vector3 toCenter = (bounds.center - transform.position).normalized;
            desiredDirection = toCenter;
        }

        AdjustDirection(desiredDirection);
        ApplyAcceleration(desiredDirection, wanderSpeed);
    }


    private bool IsOutsideWaterBody()
    {
        return !waterBodyCollider.bounds.Contains(transform.position);
    }

    private void ReturnToWaterBody()
    {
        Vector3 toCenter = (waterBodyCollider.bounds.center - transform.position).normalized;
        AdjustDirection(toCenter);
        ApplyAcceleration(toCenter, 2 * wanderSpeed);
    }

    private void TurnAwayFromBoundary()
    {
        Bounds bounds = waterBodyCollider.bounds;

        Vector3 toCenter = (bounds.center - transform.position).normalized;
        Vector3 randomOffset = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f));
        Vector3 desiredDirection = (toCenter + randomOffset).normalized;

        AdjustDirection(desiredDirection);
    }




    private void SteerInsideWaterBody()
    {
        Bounds bounds = waterBodyCollider.bounds;
        if (!bounds.Contains(transform.position))
        {
            Vector3 toCenter = (bounds.center - transform.position).normalized;
            currentVelocity += toCenter * 3 * wanderSpeed; // Strong push towards center
            ApplyAcceleration(toCenter, 3 * wanderSpeed);
        }
    }



    private bool IsNearBoundary()
    {
        Bounds bounds = waterBodyCollider.bounds;
        float margin = 1.0f; // This value can be adjusted based on the size of your fish and water body

        return transform.position.x <= bounds.min.x + margin || transform.position.x >= bounds.max.x - margin ||
               transform.position.y <= bounds.min.y + margin || transform.position.y >= bounds.max.y - margin ||
               transform.position.z <= bounds.min.z + margin || transform.position.z >= bounds.max.z - margin;
    }


    private bool IsObstacleAhead()
    {
        Vector3 forward = transform.forward;
        float rayLength = 3.0f;
        RaycastHit hit;

        if (Physics.Raycast(transform.position, forward, out hit, rayLength))
        {
            return true;
        }
        return false;
    }

    private void AvoidObstacle()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 3.0f))
        {
            Vector3 avoidDirection = Vector3.Cross(hit.normal, Vector3.up);
            AdjustDirection(avoidDirection);
            ApplyAcceleration(avoidDirection, wanderSpeed);
            ApplyVelocity();
        }
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
        if (idleTimer <= 0)
        {
            idleTimer = idleDuration;
        }
        else
        {
            idleTimer -= Time.deltaTime;
            ApplyAcceleration(-currentVelocity, 0.5f * wanderSpeed); // Slowly decelerate
        }
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
        float surfaceSpeed = 0.5f;
        Vector3 surfaceDirection = Vector3.up;

        Vector3 targetPosition = new Vector3(
            transform.position.x,
            Mathf.Min(waterBodyCollider.bounds.max.y - 0.2f, transform.position.y + 0.1f),
            transform.position.z
        );

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, surfaceSpeed * Time.deltaTime);
        if (transform.position.y >= waterBodyCollider.bounds.max.y - 0.2f)
        {
            currentSurfaceTimer = 0;  // Reset timer if fish is close to the surface
        }
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

    private bool AmIPredator()
    {
        return isPredator;
    }

    private void SeekFood()
    {
        if (foodSource != null)
        {
            Vector3 foodDirection = (foodSource.transform.position - transform.position).normalized;
            AdjustDirection(foodDirection);
            ApplyAcceleration(foodDirection, chaseSpeed);
            ApplyVelocity();
        }
    }

    private void ApplyAcceleration(Vector3 targetDirection, float speed)
    {
        targetDirection.y = 0;  // Zero out the vertical component
        currentVelocity = Vector3.MoveTowards(currentVelocity, targetDirection * speed, acceleration * Time.deltaTime);
    }


    private void ApplyVelocity()
    {
        transform.Translate(currentVelocity * Time.deltaTime, Space.World);
        if (currentVelocity.magnitude > 0.1f) // Only adjust if there's significant movement
        {
            Quaternion targetRotation = Quaternion.LookRotation(currentVelocity.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }



    private void Flee()
    {
        FishAIBehavior nearestPredator = FindNearestPredator();

        if (nearestPredator != null)
        {
            Vector3 fleeDirection = transform.position - nearestPredator.transform.position;
            fleeDirection.Normalize();
            AdjustDirection(fleeDirection);
            ApplyAcceleration(fleeDirection, fleeSpeed);
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
            AdjustDirection(chaseDirection);
            ApplyAcceleration(chaseDirection, chaseSpeed);
            ApplyVelocity();
        }
    }

    private void Wander()
    {
        if (Random.value < 0.05f) // 5% chance to change direction
        {
            Vector3 randomDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(-0.2f, 0.2f), Random.Range(-1f, 1f)).normalized;  // Reduced vertical range
            AdjustDirection(randomDirection);
            ApplyAcceleration(randomDirection, wanderSpeed);
        }
    }



    private void AdjustDirection(Vector3 newDirection)
    {
        Vector3 blendedDirection = Vector3.Lerp(transform.forward, newDirection, 0.05f);
        Quaternion targetRotation = Quaternion.LookRotation(blendedDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 0.2f * rotationSpeed * Time.deltaTime);
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
