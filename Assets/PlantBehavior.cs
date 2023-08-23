using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantBehavior : MonoBehaviour
{
    public string plantName;
    public WaterQualityParameters waterQualityManager;
    public GameObject deathIndicator;
    public GameObject wiltingIndicator;
    public Plant plantData;

    private bool isCollidingWithWater = false;
    private JSONLoader jsonLoader;

    private bool isWilting = false;
    private float wiltingTimer = 0f;
    private float lightConsumptionRate = 0.1f;
    private float nutrientConsumptionRate = 0.1f;
    private ResourcePool resourcePoolInstance;
    private float health;
    private float growthRate;

    private void Start()
    {
        jsonLoader = FindObjectOfType<JSONLoader>();
        if (jsonLoader == null)
        {
            Debug.LogError("JSONLoader not found in the scene. Make sure it exists.");
            return;
        }

        plantData = jsonLoader.GetPlantDataByName(plantName);
        if (plantData == null)
        {
            Debug.LogError("Plant data not found for: " + plantName);
            return;
        }

        if (waterQualityManager == null)
        {
            Debug.LogError("WaterQualityManager reference is not set for plant: " + plantName);
            return;
        }

        if (string.IsNullOrEmpty(plantName))
        {
            Debug.LogError("PlantName is not set for PlantBehavior script on GameObject: " + gameObject.name);
            return;
        }
    }

    private void Update()
    {
        if (isCollidingWithWater)
        {
            ApplyWaterEffects(waterQualityManager.GetpH(), waterQualityManager.GetAmmoniaLevel(), waterQualityManager.GetNitrateLevel());

            if (plantData.health <= 0 && !isWilting)
            {
                StartWilting();
            }

            if (isWilting)
            {
                UpdateWilting();
            }
        }

        if (resourcePoolInstance != null)
        {
            // Adjusted this section to ensure resourcePoolInstance is not null before using it
            float consumedLight = resourcePoolInstance.ConsumeResource(ref resourcePoolInstance.lightAvailability, lightConsumptionRate * Time.deltaTime);
            float consumedNutrient = resourcePoolInstance.ConsumeResource(ref resourcePoolInstance.nutrientAvailability, nutrientConsumptionRate * Time.deltaTime);
            UpdatePlantBehavior(consumedLight, consumedNutrient);
        }
    }

    public void UpdatePlantBehavior(float consumedLight, float consumedNutrient)
    {
        AdjustGrowthRate(consumedLight, consumedNutrient);
        AdjustHealth(consumedLight, consumedNutrient);
        // Your other adjustments and logic go here
    }

    public void ApplyWaterEffects(float pHValue, float ammoniaValue, float nitrateValue)
    {
        if (plantData != null)
        {
            if (pHValue < plantData.pH[0] || pHValue > plantData.pH[1])
            {
                plantData.health -= 5;
            }

            if (ammoniaValue > plantData.ammonia_ppm[1])
            {
                plantData.health -= 10;
            }

            if (nitrateValue > plantData.nitrate_ppm[1])
            {
                plantData.health -= 5;
            }

            plantData.health = Mathf.Clamp(plantData.health, 0, 100);
        }
        else
        {
            Debug.LogWarning("Invalid plant data");
        }
    }

    private void Die()
    {
        gameObject.SetActive(false);

        if (wiltingIndicator != null)
        {
            Instantiate(wiltingIndicator, transform.position, Quaternion.identity);
        }

        Debug.Log("Plant died: " + plantData.name);
    }

    public void StartWilting()
    {
        isWilting = true;
        wiltingTimer = 30f;
    }

    public void UpdateWilting()
    {
        wiltingTimer -= Time.deltaTime;
        if (wiltingTimer <= 0f)
        {
            Die();
        }
    }

    private float CalculateConsumedLight(PlantBehavior plantBehavior)
    {
        float consumedLight = plantBehavior.lightConsumptionRate * Time.deltaTime;
        return consumedLight;
    }

    private float CalculateConsumedNutrient(PlantBehavior plantBehavior)
    {
        float consumedNutrient = plantBehavior.nutrientConsumptionRate * Time.deltaTime;
        return consumedNutrient;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            isCollidingWithWater = true;
            Debug.Log("Plant immersed in water: " + plantName);
        }
    }

    private void AdjustGrowthRate(float consumedLight, float consumedNutrient)
    {
        // Modify growth rate based on consumed resources
        float growthRateModifier = CalculateGrowthRateModifier(consumedLight, consumedNutrient);
        growthRate += growthRateModifier;
    }

    private void AdjustHealth(float consumedLight, float consumedNutrient)
    {
        // Modify health based on consumed resources
        float healthModifier = CalculateHealthModifier(consumedLight, consumedNutrient);
        health += healthModifier;
    }

    private float CalculateGrowthRateModifier(float consumedLight, float consumedNutrient)
    {
        // Calculate a modifier for growth rate based on consumed resources
        return 0.0f; // Implement your logic here
    }

    private float CalculateHealthModifier(float consumedLight, float consumedNutrient)
    {
        // Calculate a modifier for health based on consumed resources
        return 0.0f; // Implement your logic here
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            isCollidingWithWater = false;
            Debug.Log("Plant removed from water: " + plantName);
        }
    }
}
