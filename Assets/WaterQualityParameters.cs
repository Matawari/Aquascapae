using UnityEngine;

public class WaterQualityParameters : MonoBehaviour
{
    [Header("Water Quality Parameters")]
    public float maxAmmoniaLevel = 1.0f;
    public float minOxygenLevel = 0.0f;
    public float maxOxygenLevel = 10.0f;
    public float maxNitrateLevel = 1.0f;
    public float maxNitriteLevel = 1.0f;
    public float bacteriaPopulation = 1000.0f;

    [Header("Water Parameters")]
    [SerializeField] private float nitrate = 0.0f;
    [SerializeField] private float potassium = 0.0f;
    [SerializeField] private float phosphorus = 0.0f;
    [SerializeField] private float temperature = 25.0f;
    [SerializeField] private float pH = 7.0f;
    [SerializeField] private float wasteLevel = 100.0f;
    [SerializeField] private float nutrientLevel = 0.0f;
    [SerializeField] private float algaePopulation = 0.0f;
    [SerializeField] private float ammoniaLevel = 0.0f;
    [SerializeField] private float oxygenLevel = 100.0f;
    [SerializeField] private float nitrite = 0.0f;

    [SerializeField] private WaterBody waterBody;

    // Add this property for AlgaePopulation.
    public float AlgaePopulation { get; set; }

    // Add this property for BacteriaPopulation.
    public float BacteriaPopulation { get; set; }
    

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
        float maxOxygenProduction = 10.0f;
        float algaeFactor = 0.1f;
        float temperatureFactor = 0.2f;
        float nutrientFactor = 0.05f;

        float algaeOxygen = algaePopulation * algaeFactor;
        float temperatureOxygen = Mathf.Max(0, (temperature - 25.0f) * temperatureFactor);
        float nutrientOxygen = Mathf.Max(0, nutrientLevel * nutrientFactor);

        float totalOxygenProduction = Mathf.Clamp(algaeOxygen + temperatureOxygen + nutrientOxygen, 0.0f, maxOxygenProduction);

        return totalOxygenProduction;
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
        nitrate -= waterBody.NutrientUptakeRate;
        potassium -= waterBody.NutrientUptakeRate;
        phosphorus -= waterBody.NutrientUptakeRate;
    }

    private void SimulateNutrientRelease()
    {
        nitrate += 0.2f;
        potassium += 0.2f;
        phosphorus += 0.2f;
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

    public float MaxBacteriaPopulation
    {
        get
        {
            if (waterBody != null)
            {
                return (waterBody.Width * waterBody.Length * waterBody.Depth) * 10f;
            }
            else
            {
                Debug.LogError("WaterBody is not assigned! Ensure the WaterBody component is attached to the GameObject.");
                return 10000.0f;
            }
        }
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

    public float GetCO2AbsorptionRate()
    {
        float baseAbsorptionRate = 1.0f;
        float algaeFactor = Mathf.Clamp01(algaePopulation / 1000.0f);
        float temperatureFactor = Mathf.Clamp01(1.0f - (temperature - 25.0f) * 0.02f);
        float pHFactor = pH < 7.0f ? pH / 7.0f : 1.0f;
        float wasteFactor = Mathf.Clamp01(1.0f - wasteLevel * 0.01f);
        float absorptionRate = baseAbsorptionRate * algaeFactor * temperatureFactor * pHFactor * wasteFactor;
        float randomFactor = Random.Range(0.8f, 1.2f);
        return absorptionRate * randomFactor;
    }
}
