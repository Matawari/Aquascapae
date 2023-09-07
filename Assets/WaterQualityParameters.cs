using UnityEngine;

public class WaterQualityParameters : MonoBehaviour
{
    public float maxAmmoniaLevel = 1.0f;
    public float minOxygenLevel = 0.0f;
    public float maxOxygenLevel = 10.0f;
    public float maxNitrateLevel = 1.0f;
    public float maxNitriteLevel = 1.0f;

    private float nitrate = 0.0f;
    private float potassium = 0.0f;
    private float phosphorus = 0.0f;
    private float temperature = 25.0f;
    private float pH = 7.0f;
    public float wasteLevel = 100.0f;
    public float nutrientLevel = 0.0f;
    public float bacteriaPopulation = 1000.0f;
    public float algaePopulation = 0.0f;
    private float ammoniaLevel = 0.0f;
    private float oxygenLevel = 100.0f;
    private float nitrite = 0.0f;

    [SerializeField]
    private WaterBody waterBody; // Reference to the WaterBody component

    public void AdjustWaterQualityBasedOnSubstrate(Substrate substrate)
    {
        pH += substrate.pH_effect;
        ammoniaLevel += substrate.interactions.water.effectOnAmmonia;
    }

    public void AdjustNutrientLevelsBasedOnSubstrate(Substrate substrate)
    {
        nitrate += substrate.interactions.water.effectOnNitrate;
    }

    public void UpdateParameters()
    {
        SimulateTemperatureChange();
        SimulatePHChange();
        SimulateAmmoniaChange();
        SimulateOxygenChange();
    }

    public void UpdateNutrientLevels()
    {
        SimulateNutrientUptake();
        SimulateNutrientRelease();
    }

    public float GetpH()
    {
        return pH;
    }

    public float GetAmmoniaLevel()
    {
        return ammoniaLevel;
    }

    public float GetNitriteLevel()
    {
        return nitrite;
    }

    public float GetNitrateLevel()
    {
        return nitrate;
    }

    public float GetOxygenLevel()
    {
        return oxygenLevel;
    }

    public float GetOxygenProduction()
    {
        // Constants for adjusting oxygen production
        float maxOxygenProduction = 10.0f; // Maximum oxygen production rate
        float algaeFactor = 0.1f; // Influence of algae population
        float temperatureFactor = 0.2f; // Influence of water temperature
        float nutrientFactor = 0.05f; // Influence of nutrient levels

        // Calculate algae-based oxygen production
        float algaeOxygen = algaePopulation * algaeFactor;

        // Calculate temperature-based oxygen production
        float temperatureOxygen = Mathf.Max(0, (temperature - 25.0f) * temperatureFactor);

        // Calculate nutrient-based oxygen production
        float nutrientOxygen = Mathf.Max(0, nutrientLevel * nutrientFactor);

        // Combine the factors and limit to the maximum production rate
        float totalOxygenProduction = Mathf.Clamp(algaeOxygen + temperatureOxygen + nutrientOxygen, 0.0f, maxOxygenProduction);

        return totalOxygenProduction;
    }

    // Property to calculate and return the maximum allowed bacteria population
    public float MaxBacteriaPopulation
    {
        get
        {
            if (waterBody != null)
            {
                // Calculate the maximum allowed bacteria population based on water body dimensions
                float maxPopulation = (waterBody.Width * waterBody.Length * waterBody.Depth) / 1000f;
                return maxPopulation;
            }
            else
            {
                // Handle the case where waterBody is not assigned
                Debug.LogError("WaterBody is not assigned! Ensure the WaterBody component is attached to the GameObject.");
                return 0.0f;
            }
        }
    }

    public float GetCO2AbsorptionRate()
    {
        float baseAbsorptionRate = CalculateCO2BaseAbsorptionRate();
        float randomFactor = Random.Range(0.8f, 1.2f);
        float calculatedCO2AbsorptionRate = baseAbsorptionRate * randomFactor;
        return calculatedCO2AbsorptionRate;
    }

    public float GetTemperature()
    {
        return temperature;
    }

    public void AdjustAmmoniaLevel(float amount)
    {
        ammoniaLevel += amount;
    }

    public void AdjustNitrateLevel(float amount)
    {
        nitrate += amount;
    }

    public void AdjustNitriteLevel(float amount)
    {
        nitrite += amount;
    }

    public void AdjustpHLevel(float amount)
    {
        pH += amount;
    }

    public void AdjustBacteriaPopulation(float amount)
    {
        bacteriaPopulation += amount;
    }

    public float GetWasteLevel()
    {
        return wasteLevel;
    }

    public void AdjustWasteLevel(float amount)
    {
        wasteLevel += amount;
    }

    private void SimulateTemperatureChange()
    {
        temperature += Random.Range(-1.0f, 1.0f);
        nitrate += temperature > 28.0f ? 0.1f : -0.1f;
        potassium += temperature > 28.0f ? 0.1f : -0.1f;
        phosphorus += temperature > 28.0f ? 0.1f : -0.1f;
    }

    private void SimulatePHChange()
    {
        pH += Random.Range(-0.1f, 0.1f);
        nitrate += pH < 6.5f ? 0.1f : -0.1f;
        potassium += pH < 6.5f ? 0.1f : -0.1f;
        phosphorus += pH < 6.5f ? 0.1f : -0.1f;
    }

    private void SimulateAmmoniaChange()
    {
        ammoniaLevel += Random.Range(-0.1f, 0.1f);
        ammoniaLevel = Mathf.Clamp(ammoniaLevel, 0.0f, maxAmmoniaLevel);
    }

    private void SimulateOxygenChange()
    {
        oxygenLevel += Random.Range(-0.1f, 0.1f);
        oxygenLevel = Mathf.Clamp(oxygenLevel, minOxygenLevel, maxOxygenLevel);
    }

    private void SimulateNutrientUptake()
    {
        nitrate -= 0.1f;
        potassium -= 0.1f;
        phosphorus -= 0.1f;
    }

    private void SimulateNutrientRelease()
    {
        nitrate += 0.2f;
        potassium += 0.2f;
        phosphorus += 0.2f;
    }

    private float CalculateCO2BaseAbsorptionRate()
    {
        float baseRate = 0.0f;
        return baseRate;
    }

    public void ReduceNutrientLevels(float nutrientUptakeRate, float amount)
    {
        nitrate -= nutrientUptakeRate * amount;
        potassium -= nutrientUptakeRate * amount;
        phosphorus -= nutrientUptakeRate * amount;
    }

    public void BacterialConversion()
    {
        float conversionRate = 0.1f;
        float convertedWaste = wasteLevel * conversionRate;
        wasteLevel -= convertedWaste;
        nutrientLevel += convertedWaste;
        bacteriaPopulation += convertedWaste;
    }

    public void UpdateBacteriaPopulation()
    {
        float growthRate = 0.05f;
        bacteriaPopulation += bacteriaPopulation * growthRate * nutrientLevel;
    }

    public void AdjustAlgaePopulation(float amount)
    {
        algaePopulation += amount;
    }

    public float GetAlgaePopulation()
    {
        return algaePopulation;
    }
}
