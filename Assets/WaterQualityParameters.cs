using UnityEngine;

public class WaterQualityParameters : MonoBehaviour
{
    [Header("Water Quality Parameters")]
    public float maxAmmoniaLevel = 1.0f;
    public float minOxygenLevel = 0.0f;
    public float maxOxygenLevel = 10.0f;
    public float maxNitrateLevel = 1.0f;
    public float maxNitriteLevel = 1.0f;
    public float maxPhosphorusLevel = 1.0f;
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

    public float AlgaePopulation { get; set; }
    public float BacteriaPopulation { get; set; }

    private float updateInterval = 1f;
    private float timeSinceLastUpdate = 0f;

    private void Update()
    {
        timeSinceLastUpdate += Time.deltaTime;
        if (timeSinceLastUpdate >= updateInterval)
        {
            UpdateParameters();
            UpdateNutrientLevels();
            timeSinceLastUpdate = 0f;
        }
    }

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

    // Methods to get current and forecasted values for each parameter
    public float GetCurrentNitrate() => nitrate;
    public float GetForecastedNitrate(float percentage) => nitrate - (nitrate * percentage);

    public float GetCurrentPotassium() => potassium;
    public float GetForecastedPotassium(float percentage) => potassium - (potassium * percentage);

    public float GetCurrentPhosphorus() => phosphorus;
    public float GetForecastedPhosphorus(float percentage) => phosphorus - (phosphorus * percentage);

    public float GetCurrentpH() => pH;
    public float GetForecastedpH(float percentage) => pH; // Assuming pH doesn't change with water change

    public float GetCurrentAmmonia() => ammoniaLevel;
    public float GetForecastedAmmonia(float percentage) => ammoniaLevel - (ammoniaLevel * percentage);

    public float GetCurrentOxygen() => oxygenLevel;
    public float GetForecastedOxygen(float percentage) => oxygenLevel - (oxygenLevel * percentage);

    public float GetCurrentNitrite() => nitrite;
    public float GetForecastedNitrite(float percentage) => nitrite - (nitrite * percentage);
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
        temperature += Random.Range(-1.0f, 1.0f) * Time.timeScale;
        nitrate += (temperature > 28.0f ? 0.1f : -0.1f) * Time.timeScale;
        potassium += (temperature > 28.0f ? 0.1f : -0.1f) * Time.timeScale;
        phosphorus += (temperature > 28.0f ? 0.1f : -0.1f) * Time.timeScale;
    }

    private void SimulatePHChange()
    {
        pH += Random.Range(-0.1f, 0.1f) * Time.timeScale;
        nitrate += (pH < 6.5f ? 0.1f : -0.1f) * Time.timeScale;
        potassium += (pH < 6.5f ? 0.1f : -0.1f) * Time.timeScale;
        phosphorus += (pH < 6.5f ? 0.1f : -0.1f) * Time.timeScale;
    }

    private void SimulateAmmoniaChange()
    {
        ammoniaLevel += Random.Range(-0.1f, 0.1f) * Time.timeScale;
        ammoniaLevel = Mathf.Clamp(ammoniaLevel, 0.0f, maxAmmoniaLevel);
    }

    private void SimulateOxygenChange()
    {
        oxygenLevel += Random.Range(-0.1f, 0.1f) * Time.timeScale;
        oxygenLevel = Mathf.Clamp(oxygenLevel, minOxygenLevel, maxOxygenLevel);
    }

    private void SimulateNutrientUptake()
    {
        nitrate -= waterBody.NutrientUptakeRate * Time.timeScale;
        potassium -= waterBody.NutrientUptakeRate * Time.timeScale;
        phosphorus -= waterBody.NutrientUptakeRate * Time.timeScale;
    }

    private void SimulateNutrientRelease()
    {
        nitrate += 0.2f * Time.timeScale;
        potassium += 0.2f * Time.timeScale;
        phosphorus += 0.2f * Time.timeScale;
    }

    public void ReduceNutrientLevels(float nutrientUptakeRate, float amount)
    {
        nitrate -= nutrientUptakeRate * amount * Time.timeScale;
        potassium -= nutrientUptakeRate * amount * Time.timeScale;
        phosphorus -= nutrientUptakeRate * amount * Time.timeScale;
    }

    public void BacterialConversion()
    {
        float conversionRate = 0.1f;
        float convertedWaste = wasteLevel * conversionRate * Time.timeScale;
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
        bacteriaPopulation += bacteriaPopulation * growthRate * nutrientLevel * Time.timeScale;
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

    public void AdjustNutrientLevels(float amount)
    {
        // Define the percentage of nutrients released from decomposed organic matter.
        float ammoniaReleasePercentage = 0.5f; // 50% of the decomposed matter becomes ammonia.
        float nitrateReleasePercentage = 0.3f; // 30% becomes nitrate.
        float phosphateReleasePercentage = 0.2f; // 20% becomes phosphate.

        // Calculate the amount of each nutrient released.
        float ammoniaReleased = amount * ammoniaReleasePercentage;
        float nitrateReleased = amount * nitrateReleasePercentage;
        float phosphateReleased = amount * phosphateReleasePercentage;

        // Adjust the nutrient levels in the water.
        ammoniaLevel += ammoniaReleased;
        nitrate += nitrateReleased; // Changed from nitrateLevel to nitrate
        phosphorus += phosphateReleased; // Changed from phosphateLevel to phosphorus

        // Ensure nutrient levels don't exceed their maximum values.
        ammoniaLevel = Mathf.Clamp(ammoniaLevel, 0, maxAmmoniaLevel);
        nitrate = Mathf.Clamp(nitrate, 0, maxNitrateLevel);
        // Assuming you have a maxPhosphorusLevel defined
        phosphorus = Mathf.Clamp(phosphorus, 0, maxPhosphorusLevel);
    }


}
