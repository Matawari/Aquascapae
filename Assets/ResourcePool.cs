using UnityEngine;

public class ResourcePool : MonoBehaviour
{
    public float nutrientAvailability = 1000.0f; // Updated nutrient availability
    public float lightAvailability = 1000.0f; // Updated light availability

    public float ConsumeResource(ref float resource, float amount)
    {
        float consumed = Mathf.Min(resource, amount);
        resource -= consumed;
        return consumed;
    }

    public void ReleaseResource(ref float resource, float amount)
    {
        resource += amount;
    }

    public void AdjustNutrientAvailability(float amount)
    {
        // Implement logic to adjust the nutrient availability
        nutrientAvailability += amount;
    }

    public float GetNutrientAvailability()
    {
        return nutrientAvailability;
    }

    public float GetLightAvailability()
    {
        return lightAvailability;
    }
}
