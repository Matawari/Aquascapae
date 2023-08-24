using UnityEngine;
using System.Collections.Generic;

public class FishAIBehavior : MonoBehaviour
{
    public Transform[] hidingSpots;
    public GameObject foodSource;

    private bool isPredator = false;
    private float hungerTimer = 30.0f;
    private float currentHungerTimer;
    private float detectionRadius = 5.0f;

    private BehaviorTreeNode rootBehavior;

    private void Start()
    {
        currentHungerTimer = hungerTimer;

        rootBehavior = new SelectorNode(
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
        rootBehavior.Execute();
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

    private void Flee()
    {
        FishAIBehavior nearestPredator = FindNearestPredator();

        if (nearestPredator != null)
        {
            Vector3 fleeDirection = transform.position - nearestPredator.transform.position;
            fleeDirection.Normalize();

            // Implement your own flee speed value here
            float fleeSpeed = 5.0f; // Adjust this value as needed

            transform.Translate(fleeDirection * fleeSpeed * Time.deltaTime);
        }
    }

    private void Chase()
    {
        FishAIBehavior targetFish = FindTargetFish();

        if (targetFish != null)
        {
            Vector3 chaseDirection = targetFish.transform.position - transform.position;
            chaseDirection.Normalize();

            // Implement your own chase speed value here
            float chaseSpeed = 3.0f; // Adjust this value as needed

            transform.Translate(chaseDirection * chaseSpeed * Time.deltaTime);
        }
    }

    private void Wander()
    {
        // Implement your own wander behavior here
        // For example, move in a random direction within a certain radius

        Vector3 randomDirection = Random.insideUnitSphere * detectionRadius;
        randomDirection += transform.position;

        // Implement your own wander speed value here
        float wanderSpeed = 1.0f; // Adjust this value as needed

        // Ensure the fish stays within bounds of the tank or environment
        // Make sure to handle the case when NavMesh is not used

        // For example:
        transform.Translate((randomDirection - transform.position).normalized * wanderSpeed * Time.deltaTime);
    }


    private FishAIBehavior FindNearestPredator()
    {
        FishAIBehavior nearestPredator = null;
        float nearestDistance = float.MaxValue;

        Collider[] nearbyObjects = Physics.OverlapSphere(transform.position, detectionRadius);

        foreach (var obj in nearbyObjects)
        {
            FishAIBehavior otherFish = obj.GetComponent<FishAIBehavior>();

            if (otherFish != null && otherFish.isPredator)
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

            if (otherFish != null && !otherFish.isPredator && otherFish != this)
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
