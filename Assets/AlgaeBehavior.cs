using UnityEngine;

public class AlgaeBehavior : MonoBehaviour
{
    public WaterQualityParameters waterQualityParameters;
    public ResourcePool resourcePool; // Add this line

    private float bloomThreshold = 2.0f; // Example threshold for algae bloom

    public void UpdateAlgae()
    {
        ApplyAlgaeEffects();
        SimulateAlgaeGrowth();
        AlgalResourceCycling();
        CheckForBloom();
    }

    private void ApplyAlgaeEffects()
    {
        // For example, adjust nutrient levels and affect other organisms' health
        float pHEffect = 0.05f;       // Example effect on pH levels
        float nitrateEffect = -0.1f;  // Example effect on nitrate levels

        waterQualityParameters.AdjustpHLevel(pHEffect);
        waterQualityParameters.AdjustNitrateLevel(nitrateEffect);
    }

    private void SimulateAlgaeGrowth()
    {
        // Simulate growth based on available resources and competition
        float growthRate = 0.05f; // Example growth rate

        float nutrientAvailability = resourcePool.GetNutrientAvailability();
        float lightAvailability = resourcePool.GetLightAvailability();
        float growth = nutrientAvailability * lightAvailability * growthRate * Time.deltaTime;

        waterQualityParameters.AdjustAlgaePopulation(growth);
    }

    private void AlgalResourceCycling()
    {
        // Convert excess algae into nutrients
        float conversionRate = 0.2f; // Example conversion rate

        float algaePopulation = waterQualityParameters.GetAlgaePopulation();
        float convertedNutrients = algaePopulation * conversionRate * Time.deltaTime;

        resourcePool.AdjustNutrientAvailability(convertedNutrients);
        waterQualityParameters.AdjustAlgaePopulation(-convertedNutrients);
    }

    private void CheckForBloom()
    {
        if (waterQualityParameters.GetAlgaePopulation() >= bloomThreshold)
        {
            // Implement logic for handling algae bloom
        }
    }
}
