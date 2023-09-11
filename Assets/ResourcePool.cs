using UnityEngine;

public class ResourcePool : MonoBehaviour
{
    [SerializeField] private float lightAvailability = 100.0f; // Assuming a default value
    public float nutrientAvailability = 1000.0f; // Updated nutrient availability

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
        // If light availability is low, reduce the rate of nutrient consumption
        float lightFactor = lightAvailability / 1000.0f; // Assuming 1000 is the max light availability
        float adjustedAmount = amount * lightFactor;

        nutrientAvailability += adjustedAmount;

        // Clamping the nutrient availability between 0 and a maximum value (let's assume 2000 for now)
        nutrientAvailability = Mathf.Clamp(nutrientAvailability, 0.0f, 2000.0f);
    }


    public float GetNutrientAvailability()
    {
        return nutrientAvailability;
    }

    public float GetLightAvailability()
    {
        return lightAvailability;
    }

    public void ReduceLightAvailability(float amount)
    {
        lightAvailability -= amount;

        // Clamping the light availability between 0 and 1000
        lightAvailability = Mathf.Clamp(lightAvailability, 0.0f, 1000.0f);
    }

}
