using UnityEngine;

public class AlgaeBehavior : MonoBehaviour
{
    public WaterQualityParameters waterQualityParameters;
    public ResourcePool resourcePool;
    private float bloomThreshold = 2.0f;

    public void UpdateAlgae()
    {
        ApplyAlgaeEffects();
        SimulateAlgaeGrowth();
        AlgalResourceCycling();
        CheckForBloom();
    }

    private void ApplyAlgaeEffects()
    {
        float pHEffect = 0.05f;
        float nitrateEffect = -0.1f;

        waterQualityParameters.AdjustpHLevel(pHEffect);
        waterQualityParameters.AdjustNitrateLevel(nitrateEffect);
    }

    private void SimulateAlgaeGrowth()
    {
        float growthRate = 0.05f;

        float nutrientAvailability = resourcePool.GetNutrientAvailability();
        float lightAvailability = resourcePool.GetLightAvailability();
        float growth = nutrientAvailability * lightAvailability * growthRate * Time.deltaTime;

        waterQualityParameters.AdjustAlgaePopulation(growth);
    }

    private void AlgalResourceCycling()
    {
        float conversionRate = 0.2f;

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
