using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterQualityParameters : MonoBehaviour
{
    public float maxAmmoniaLevel = 1.0f; // Adjust the value as needed
    public float minOxygenLevel = 0.0f;  // Adjust the value as needed
    public float maxOxygenLevel = 10.0f; // Adjust the value as needed

    private float nitrate = 0.0f;
    private float potassium = 0.0f;
    private float phosphorus = 0.0f;
    private float temperature = 25.0f; // in Celsius
    private float pH = 7.0f; // Neutral
    public float wasteLevel = 100.0f; // example waste level, adjust as needed
    public float nutrientLevel = 0.0f; // example nutrient level, adjust as needed
    public float bacteriaPopulation = 1000.0f; // example bacteria population, adjust as needed
    public float algaePopulation = 0.0f; // Example algae population
    public float GetNitriteLevel() => nitrate;
    public float GetNitrateLevel() => nitrate;

    private float ammoniaLevel = 0.0f; // Ammonia concentration
    private float oxygenLevel = 100.0f; // Oxygen concentration

    public float GetpH()
    {
        return pH;
    }

    public float GetOxygenProduction()
    {
        // Implement logic to calculate and return oxygen production
        return 0.0f; // Placeholder value, adjust as needed
    }

    public float GetCO2AbsorptionRate()
    {
        // Implement logic to calculate and return CO2 absorption rate
        return 0.0f; // Placeholder value, adjust as needed
    }

    public float GetAmmoniaLevel()
    {
        // You can implement the logic to calculate and return the ammonia level here
        // For example:
        return Mathf.Max(nitrate, potassium, phosphorus);
    }
    public float GetOxygenLevel()
    {
        // Implement logic to calculate and return the oxygen level here
        return oxygenLevel;
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

    public void AdjustAlgaePopulation(float amount)
    {
        // Implement logic to adjust the algae population
        algaePopulation += amount;
    }

    public float GetAlgaePopulation()
    {
        return algaePopulation;
    }

    public void BacterialConversion()
    {
        float conversionRate = 0.1f; // example conversion rate, adjust as needed
        float convertedWaste = wasteLevel * conversionRate;
        wasteLevel -= convertedWaste;
        nutrientLevel += convertedWaste;
        bacteriaPopulation += convertedWaste;
    }

    public void UpdateBacteriaPopulation()
    {
        float growthRate = 0.05f; // example growth rate, adjust as needed
        bacteriaPopulation += bacteriaPopulation * growthRate * nutrientLevel;
    }

    public void AdjustBacteriaPopulation(float amount)
    {
        // Implement logic to adjust the bacteria population
        bacteriaPopulation += amount;
    }

    public float GetWasteLevel()
    {
        return wasteLevel;
    }

    public void AdjustWasteLevel(float amount)
    {
        // Implement logic to adjust the waste level
        wasteLevel += amount;
    }

    private void SimulateTemperatureChange()
    {
        // Simulate daily temperature changes (e.g., based on day-night cycle)
        temperature += Random.Range(-1.0f, 1.0f); // change by up to 1 degree

        // Adjust nitrate, potassium, phosphorus levels accordingly
        nitrate += temperature > 28.0f ? 0.1f : -0.1f;
        potassium += temperature > 28.0f ? 0.1f : -0.1f;
        phosphorus += temperature > 28.0f ? 0.1f : -0.1f;
    }

    private void SimulatePHChange()
    {
        // Simulate pH changes over time based on various factors (e.g., respiration, decomposition)
        pH += Random.Range(-0.1f, 0.1f); // change by up to 0.1 units

        // Adjust nitrate, potassium, phosphorus levels accordingly
        nitrate += pH < 6.5f ? 0.1f : -0.1f;
        potassium += pH < 6.5f ? 0.1f : -0.1f;
        phosphorus += pH < 6.5f ? 0.1f : -0.1f;
    }

    private void SimulateAmmoniaChange()
    {
        // Simulate changes in ammonia level over time based on various factors
        ammoniaLevel += Random.Range(-0.1f, 0.1f); // change by up to 0.1 units
        ammoniaLevel = Mathf.Clamp(ammoniaLevel, 0.0f, maxAmmoniaLevel); // Clamp within valid range
    }

    private void SimulateOxygenChange()
    {
        // Simulate changes in oxygen level over time based on various factors
        oxygenLevel += Random.Range(-0.1f, 0.1f); // change by up to 0.1 units
        oxygenLevel = Mathf.Clamp(oxygenLevel, minOxygenLevel, maxOxygenLevel); // Clamp within valid range
    }

    private void SimulateNutrientUptake()
    {
        // Simulate nutrient uptake by plants over time
        nitrate -= 0.1f;
        potassium -= 0.1f;
        phosphorus -= 0.1f;
    }

    private void SimulateNutrientRelease()
    {
        // Simulate nutrient release by fish or other factors over time
        nitrate += 0.2f;
        potassium += 0.2f;
        phosphorus += 0.2f;
    }

    public void AdjustNitrateLevel(float amount)
    {
        // Implement logic to adjust the nitrate level
        nitrate += amount;
    }

    public void ReduceNutrientLevels(float reductionAmount)
    {
        nitrate -= reductionAmount;
        potassium -= reductionAmount;
        phosphorus -= reductionAmount;

        nitrate = Mathf.Max(nitrate, 0.0f);
        potassium = Mathf.Max(potassium, 0.0f);
        phosphorus = Mathf.Max(phosphorus, 0.0f);
    }
    public void AdjustAmmoniaLevel(float amount)
    {
        // Implement logic to adjust the ammonia level
        ammoniaLevel += amount;
    }

    public void AdjustpHLevel(float amount)
    {
        // Implement logic to adjust the pH level
        pH += amount;
    }
    public float GetPotassiumLevel() => potassium;
    public float GetPhosphorusLevel() => phosphorus;
    public float GetTemperature() => temperature;
    public float GetPH() => pH;
}
