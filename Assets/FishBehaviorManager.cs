using UnityEngine;
using System.Collections.Generic;

public class FishBehaviorManager : MonoBehaviour
{
    public WaterQualityParameters waterQualityParameters;
    private List<FishBehavior> fishBehaviors;

    private void Start()
    {
        fishBehaviors = new List<FishBehavior>(FindObjectsOfType<FishBehavior>());
    }

    private void Update()
    {
        SimulateFishBehaviors();
    }

    private void SimulateFishBehaviors()
    {
        foreach (FishBehavior fishBehavior in fishBehaviors)
        {
            fishBehavior.Grow();
            fishBehavior.Eat();
        }

    }
}
