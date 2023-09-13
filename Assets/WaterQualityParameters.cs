using UnityEngine;

public class WaterQualityParameters : MonoBehaviour
{
    [Header("Water Quality Thresholds")]
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
    [SerializeField] private float toxinLevel = 0.0f;
    [SerializeField] private WaterBody waterBody;

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
        ammoniaLevel = Mathf.Clamp(ammoniaLevel, 0, maxAmmoniaLevel);
    }

    public void AdjustNutrientLevelsBasedOnSubstrate(Substrate substrate)
    {
        nitrate += substrate.interactions.water.effectOnNitrate;
        nitrate = Mathf.Clamp(nitrate, 0, maxNitrateLevel);
    }

    public void UpdateParameters()
    {
        SimulateTemperatureChange();
        SimulatePHChange();
        SimulateAmmoniaChange();
        SimulateOxygenChange();
        NitrateNaturalDecay();
    }

    private void NitrateNaturalDecay()
    {
        nitrate -= 0.01f * nitrate * Time.deltaTime;
        nitrate = Mathf.Clamp(nitrate, 0, maxNitrateLevel);
    }

    public void UpdateNutrientLevels()
    {
        SimulateNutrientUptake();
        SimulateNutrientRelease();
    }

    private void SimulateTemperatureChange()
    {
        temperature += Random.Range(-0.5f, 0.5f) * Time.timeScale;
        nitrate += (temperature > 28.0f ? 0.01f : -0.05f) * Time.timeScale;
        potassium += (temperature > 28.0f ? 0.05f : -0.05f) * Time.timeScale;
        phosphorus += (temperature > 28.0f ? 0.05f : -0.05f) * Time.timeScale;

        nitrate = Mathf.Clamp(nitrate, 0, maxNitrateLevel);
        potassium = Mathf.Clamp(potassium, 0, 100);
        phosphorus = Mathf.Clamp(phosphorus, 0, maxPhosphorusLevel);
    }

    private void SimulatePHChange()
    {
        pH += Random.Range(-0.05f, 0.05f) * Time.timeScale;
        nitrate += (pH < 6.5f ? 0.01f : -0.05f) * Time.timeScale;
        potassium += (pH < 6.5f ? 0.05f : -0.05f) * Time.timeScale;
        phosphorus += (pH < 6.5f ? 0.05f : -0.05f) * Time.timeScale;

        nitrate = Mathf.Clamp(nitrate, 0, maxNitrateLevel);
        potassium = Mathf.Clamp(potassium, 0, 100);
        phosphorus = Mathf.Clamp(phosphorus, 0, maxPhosphorusLevel);
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

        nitrate = Mathf.Clamp(nitrate, 0, maxNitrateLevel);
        potassium = Mathf.Clamp(potassium, 0, 100);
        phosphorus = Mathf.Clamp(phosphorus, 0, maxPhosphorusLevel);
    }

    private void SimulateNutrientRelease()
    {
        nitrate += 0.02f * Time.timeScale;
        potassium += 0.1f * Time.timeScale;
        phosphorus += 0.1f * Time.timeScale;

        nitrate = Mathf.Clamp(nitrate, 0, maxNitrateLevel);
        potassium = Mathf.Clamp(potassium, 0, 100);
        phosphorus = Mathf.Clamp(phosphorus, 0, maxPhosphorusLevel);
    }

    public void ReduceNutrientLevels(float nutrientUptakeRate, float amount)
    {
        nitrate -= nutrientUptakeRate * amount * Time.timeScale;
        potassium -= nutrientUptakeRate * amount * Time.timeScale;
        phosphorus -= nutrientUptakeRate * amount * Time.timeScale;

        nitrate = Mathf.Clamp(nitrate, 0, maxNitrateLevel);
        potassium = Mathf.Clamp(potassium, 0, 100);
        phosphorus = Mathf.Clamp(phosphorus, 0, maxPhosphorusLevel);
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
        float ammoniaReleasePercentage = 0.5f;
        float nitrateReleasePercentage = 0.3f;
        float phosphateReleasePercentage = 0.2f;

        float ammoniaReleased = amount * ammoniaReleasePercentage;
        float nitrateReleased = amount * nitrateReleasePercentage;
        float phosphateReleased = amount * phosphateReleasePercentage;

        ammoniaLevel += ammoniaReleased;
        nitrate += nitrateReleased;
        phosphorus += phosphateReleased;

        ammoniaLevel = Mathf.Clamp(ammoniaLevel, 0, maxAmmoniaLevel);
        nitrate = Mathf.Clamp(nitrate, 0, maxNitrateLevel);
        phosphorus = Mathf.Clamp(phosphorus, 0, maxPhosphorusLevel);
    }

    public void AdjustpHLevel(float amount)
    {
        pH += amount;
        if (amount > 0)
        {
            ammoniaLevel += 0.05f * amount;
        }
        else
        {
            ammoniaLevel -= 0.03f * amount;
        }
        ammoniaLevel = Mathf.Clamp(ammoniaLevel, 0, maxAmmoniaLevel);
    }

    public void AdjustNitrateLevel(float amount)
    {
        nitrate += amount;
        if (amount > 0)
        {
            algaePopulation += 0.1f * amount;
        }
        else
        {
            algaePopulation += 0.05f * amount;
        }
        algaePopulation = Mathf.Clamp(algaePopulation, 0, 10000.0f);
        if (algaePopulation > 5000.0f)
        {
            oxygenLevel -= 0.05f * (algaePopulation - 5000.0f);
            oxygenLevel = Mathf.Clamp(oxygenLevel, minOxygenLevel, maxOxygenLevel);
        }
    }

    public void AdjustAmmoniaLevel(float amount)
    {
        ammoniaLevel += amount;
        if (amount > 0)
        {
            oxygenLevel -= 0.1f * amount;
        }
        else
        {
            oxygenLevel += 0.05f * amount;
        }
        oxygenLevel = Mathf.Clamp(oxygenLevel, minOxygenLevel, maxOxygenLevel);
    }

    public void AdjustOxygenLevel(float amount)
    {
        oxygenLevel += amount;
        oxygenLevel = Mathf.Clamp(oxygenLevel, 0.0f, maxOxygenLevel);
    }

    public void AdjustToxinLevel(float amount)
    {
        toxinLevel += amount;
    }

    public float GetOxygenLevel() => oxygenLevel;
    public float GetToxinLevel() => toxinLevel;

    public void AdjustBacteriaPopulation(float amount)
    {
        bacteriaPopulation += amount;
        float convertedAmmonia = 0.1f * amount;
        ammoniaLevel -= convertedAmmonia;
        nitrite += convertedAmmonia;

        float convertedNitrite = 0.1f * amount;
        nitrite -= convertedNitrite;
        nitrate += convertedNitrite * 2f;

        ammoniaLevel = Mathf.Clamp(ammoniaLevel, 0, maxAmmoniaLevel);
        nitrite = Mathf.Clamp(nitrite, 0, maxNitriteLevel);
        nitrate = Mathf.Clamp(nitrate, 0, maxNitrateLevel);
    }

    public void AdjustWasteLevel(float amount)
    {
        wasteLevel += amount;
        ammoniaLevel += 0.2f * amount;
        ammoniaLevel = Mathf.Clamp(ammoniaLevel, 0, maxAmmoniaLevel);
    }

    public float GetNutrientLevel()
    {
        return Mathf.Clamp(nutrientLevel, 0, 100);
    }

    public float GetPhosphorusLevel()
    {
        return Mathf.Clamp(phosphorus, 0, maxPhosphorusLevel);
    }

    public float GetWasteLevel()
    {
        return Mathf.Max(0, wasteLevel);
    }

    public float GetpH()
    {
        return Mathf.Clamp(pH, 0, 14);
    }

    public float GetAmmoniaLevel()
    {
        return Mathf.Clamp(ammoniaLevel, 0, maxAmmoniaLevel);
    }

    public float GetNitriteLevel()
    {
        return Mathf.Clamp(nitrite, 0, maxNitriteLevel);
    }

    public float GetNitrateLevel()
    {
        return Mathf.Clamp(nitrate, 0, maxNitrateLevel);
    }

    public float GetTemperature()
    {
        return Mathf.Clamp(temperature, -5, 100);
    }

    public void SetTemperature(float value)
    {
        temperature = Mathf.Clamp(value, -5, 100);
    }

    public void ApplyWaterChange(float changeFactor)
    {
        nitrate -= nitrate * changeFactor;
        potassium -= potassium * changeFactor;
        phosphorus -= phosphorus * changeFactor;
        ammoniaLevel -= ammoniaLevel * changeFactor;
        oxygenLevel -= oxygenLevel * changeFactor;
        nitrite -= nitrite * changeFactor;

        ammoniaLevel = Mathf.Clamp(ammoniaLevel, 0, maxAmmoniaLevel);
        oxygenLevel = Mathf.Clamp(oxygenLevel, minOxygenLevel, maxOxygenLevel);
        nitrate = Mathf.Clamp(nitrate, 0, maxNitrateLevel);
        nitrite = Mathf.Clamp(nitrite, 0, maxNitriteLevel);
        phosphorus = Mathf.Clamp(phosphorus, 0, maxPhosphorusLevel);
    }

    public float GetOxygenProduction()
    {
        float algaeOxygenProduction = algaePopulation * 0.05f;
        float bacteriaOxygenConsumption = bacteriaPopulation * 0.02f;
        return algaeOxygenProduction - bacteriaOxygenConsumption;
    }

    public float BacteriaPopulation
    {
        get { return bacteriaPopulation; }
    }

    public float AlgaePopulation
    {
        get { return algaePopulation; }
    }

    public float GetCurrentNitrate()
    {
        float bacterialConversionRate = bacteriaPopulation / MaxBacteriaPopulation;
        nitrate += (ammoniaLevel + nitrite) * bacterialConversionRate;

        float plantUptakeRate = algaePopulation / 2500.0f;

        nitrate -= nitrate * plantUptakeRate;

        return Mathf.Clamp(nitrate, 0, maxNitrateLevel);
    }

    public float GetCurrentPotassium()
    {
        float runoffFactor = temperature > 28.0f ? 0.02f : 0.01f;
        potassium += runoffFactor;

        float plantUptakeRate = algaePopulation / 10000.0f;
        potassium -= potassium * plantUptakeRate;

        return Mathf.Clamp(potassium, 0, 100);
    }

    public float GetCurrentPhosphorus()
    {
        float runoffFactor = temperature > 28.0f ? 0.03f : 0.01f;
        phosphorus += runoffFactor;

        float plantUptakeRate = algaePopulation / 10000.0f;
        phosphorus -= phosphorus * plantUptakeRate;

        return Mathf.Clamp(phosphorus, 0, maxPhosphorusLevel);
    }

    public float GetForecastedNitrate(float waterChangeAmount)
    {
        return nitrate - (nitrate * waterChangeAmount);
    }

    public float GetForecastedPotassium(float waterChangeAmount)
    {
        return potassium - (potassium * waterChangeAmount);
    }

    public float GetForecastedPhosphorus(float waterChangeAmount)
    {
        return phosphorus - (phosphorus * waterChangeAmount);
    }

    public float GetCurrentpH()
    {
        return pH;
    }

    public float GetForecastedpH(float waterChangeAmount)
    {
        return pH;
    }

    public float GetCurrentAmmonia()
    {
        return ammoniaLevel;
    }

    public float GetForecastedAmmonia(float waterChangeAmount)
    {
        return ammoniaLevel - (ammoniaLevel * waterChangeAmount);
    }

    public float GetCurrentOxygen()
    {
        return oxygenLevel;
    }

    public float GetForecastedOxygen(float waterChangeAmount)
    {
        return oxygenLevel;
    }

    public float GetCurrentNitrite()
    {
        return nitrite;
    }

    public float GetForecastedNitrite(float waterChangeAmount)
    {
        return nitrite - (nitrite * waterChangeAmount);
    }
}
