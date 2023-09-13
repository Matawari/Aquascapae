using UnityEngine;

public class LightBehaviorManager : MonoBehaviour
{
    public WaterQualityParameters waterQualityParameters;

    public string lightDataName;
    public float currentLightIntensity;

    private void Start()
    {
        if (waterQualityParameters == null)
        {
            Debug.LogError("WaterQualityParameters reference not set in LightBehaviorManager");
            enabled = false;
            return;
        }
    }

    private void Update()
    {
        // Simulate light intensity
        SimulateLightIntensity();

        // Update water quality based on the current light intensity
        UpdateWaterQualityWithArtificialLight(currentLightIntensity);
    }

    private void SimulateLightIntensity()
    {
        // Simple sinusoidal model for day-night light intensity cycle
        float amplitude = 1.0f;
        float frequency = 0.1f;
        currentLightIntensity = amplitude * Mathf.Sin(2 * Mathf.PI * frequency * Time.time);

        // Clamp light intensity to [0, 1]
        currentLightIntensity = Mathf.Clamp(currentLightIntensity, 0, 1);
    }

    private void UpdateWaterQualityWithArtificialLight(float lightIntensity)
    {
        // Adjust algae growth based on light intensity
        waterQualityParameters.AdjustAlgaePopulation(lightIntensity * 0.05f);

        // Adjust plant nutrient uptake based on light intensity
        waterQualityParameters.AdjustNitrateLevel(-lightIntensity * 0.02f);
        waterQualityParameters.AdjustNutrientLevels(-lightIntensity * 0.01f);

        // Adjust temperature based on light intensity
        AdjustTemperature(lightIntensity * 0.1f);
    }

    private void AdjustTemperature(float change)
    {
        float newTemperature = waterQualityParameters.GetTemperature() + change;
        waterQualityParameters.SetTemperature(newTemperature);
    }

}
