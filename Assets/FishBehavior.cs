using UnityEngine;

public class FishBehavior : MonoBehaviour
{
    public Fish fish;
    public WaterQualityManager waterQualityManager;
    public GameObject deadCreatureIndicator;
    public WaterQualityParameters waterQualityParameters;

    public float health = 100.0f;
    public float nutritionValue = 50.0f;

    private bool isCollidingWithWater = false;
    private JSONLoader jsonLoader;
    private FishBehavior predator;
    public ResourcePool resourcePool;
    public FishData fishData;

    private void Start()
    {
        jsonLoader = FindObjectOfType<JSONLoader>();
        if (jsonLoader == null)
        {
            Debug.LogError("JSONLoader not found in the scene. Make sure it exists.");
            return;
        }

        if (fish.predatorFoodAmount > 0)
        {
            Debug.Log(fish.name + " is a predator.");
            predator = this;
        }
        else if (fish.herbivoreFoodAmount > 0)
        {
            Debug.Log(fish.name + " is potential prey.");
        }
    }

    private void Update()
    {
        if (isCollidingWithWater)
        {
            float pHValue = waterQualityParameters.GetpH();
            float ammoniaValue = waterQualityParameters.GetAmmoniaLevel();
            float nitriteValue = waterQualityParameters.GetNitriteLevel();
            float nitrateValue = waterQualityParameters.GetNitrateLevel();
            float o2ProductionRate = waterQualityParameters.GetOxygenProduction();
            float co2AbsorptionRate = waterQualityParameters.GetCO2AbsorptionRate();
            float currentTemperature = jsonLoader.GetCurrentTemperature();

            ApplyWaterEffects(fishData, pHValue, ammoniaValue, nitriteValue, nitrateValue, o2ProductionRate, co2AbsorptionRate, currentTemperature);

            if (health <= 0)
            {
                Die();
            }

            if (fish.predatorFoodAmount > 0 && predator != null)
            {
                if (predator.gameObject.activeSelf)
                {
                    PredatorPreyInteraction();
                }
            }
        }

        ApplyBacterialEffects();
    }

    public void ApplyWaterEffects(FishData fishData, float pHValue, float ammoniaValue, float nitriteValue, float nitrateValue, float o2ProductionRate, float co2AbsorptionRate, float currentTemperature)
    {
        health -= waterQualityParameters.GetAmmoniaLevel() * 0.1f;
        health -= waterQualityParameters.GetNitrateLevel() * 0.05f;

        if (fish.isHerbivorous)
        {
            resourcePool.AdjustNutrientAvailability(-0.05f);
        }
    }

    private void PredatorPreyInteraction()
    {
        if (fish.herbivoreFoodAmount > 0)
        {
            float predationAmount = 10.0f;
            predator.health += predationAmount;
            health -= predationAmount;
        }
    }

    private void ApplyBacterialEffects()
    {
        float harmfulBacteriaThreshold = 10000.0f;
        if (waterQualityParameters.bacteriaPopulation > harmfulBacteriaThreshold)
        {
            health -= 5;
        }
    }

    private void Die()
    {
        gameObject.SetActive(false);

        if (deadCreatureIndicator != null)
        {
            Instantiate(deadCreatureIndicator, transform.position, Quaternion.identity);
        }

        Debug.Log("Creature died: " + fish.name);

        if (fish.isHerbivorous)
        {
            resourcePool.AdjustNutrientAvailability(nutritionValue);
        }
        else if (fish.predatorFoodAmount > 0)
        {
            waterQualityParameters.AdjustBacteriaPopulation(50f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            isCollidingWithWater = true;
            Debug.Log("Creature immersed in water: " + fish.name);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            isCollidingWithWater = false;
            Debug.Log("Creature removed from water: " + fish.name);
        }
    }
}
