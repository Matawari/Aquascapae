using UnityEngine;

public class PlantBehavior : MonoBehaviour
{
    public string plantName;
    public WaterQualityParameters waterQualityManager;
    public GameObject deathIndicator;
    public GameObject wiltingIndicator;
    public PlantTraits plantTraits;

    private bool isCollidingWithWater = false;
    private bool isWilting = false;
    private float wiltingTimer = 0f;
    private float lightConsumptionRate = 0.1f;
    private float nutrientConsumptionRate = 0.1f;
    private ResourcePool resourcePoolInstance;
    private float health;
    private float growthRate;

    private void Start()
    {
        plantTraits = GetComponent<PlantTraits>();

        if (plantTraits == null)
        {
            Debug.LogError("PlantTraits component not found on GameObject: " + gameObject.name);
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

            // Debug health and stress values
            Debug.Log("Health: " + plantTraits.health + ", Stress: " + plantTraits.stress);

            if (plantTraits.GetHealth() <= 0 && !isWilting)
            {
                StartWilting();
            }

            if (isWilting)
            {
                UpdateWilting();
            }
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
        if (plantTraits != null)
        {
            // Calculate the effects of pH, ammonia, and nitrate on health and stress
            float pHHealthEffect = CalculatePHEffect(pHValue);
            float ammoniaHealthEffect = CalculateAmmoniaEffect(ammoniaValue);
            float nitrateHealthEffect = CalculateNitrateEffect(nitrateValue);

            // Modify plant's health and stress based on the calculated effects
            plantTraits.health += pHHealthEffect + ammoniaHealthEffect + nitrateHealthEffect;
            plantTraits.stress += pHHealthEffect + ammoniaHealthEffect + nitrateHealthEffect;

            // Ensure health and stress stay within desired ranges, e.g., [0, 100]
            plantTraits.health = Mathf.Clamp(plantTraits.health, 0.0f, 100.0f);
            plantTraits.stress = Mathf.Clamp(plantTraits.stress, 0.0f, 100.0f);
        }
        else
        {
            Debug.LogWarning("PlantTraits reference is not set.");
        }
    }


    private float CalculatePHEffect(float pHValue)
    {
        // Implement your logic to calculate the effect of pH on health
        // For example, you can reduce health if pH is outside an optimal range
        float optimalPHRangeMin = 6.5f;
        float optimalPHRangeMax = 7.5f;

        if (pHValue < optimalPHRangeMin || pHValue > optimalPHRangeMax)
        {
            return -5.0f; // Adjust the value based on your simulation
        }
        return 0.0f; // No effect within the optimal range
    }

    private float CalculateAmmoniaEffect(float ammoniaValue)
    {
        // Implement your logic to calculate the effect of ammonia on health
        // For example, you can reduce health if ammonia exceeds a certain threshold
        float maxAmmoniaThreshold = 1.0f;

        if (ammoniaValue > maxAmmoniaThreshold)
        {
            return -10.0f; // Adjust the value based on your simulation
        }
        return 0.0f; // No effect if ammonia is within limits
    }

    private float CalculateNitrateEffect(float nitrateValue)
    {
        // Implement your logic to calculate the effect of nitrate on health
        // For example, you can reduce health if nitrate exceeds a certain threshold
        float maxNitrateThreshold = 1.0f;

        if (nitrateValue > maxNitrateThreshold)
        {
            return -5.0f; // Adjust the value based on your simulation
        }
        return 0.0f; // No effect if nitrate is within limits
    }


    private void Die()
    {
        gameObject.SetActive(false);

        if (wiltingIndicator != null)
        {
            Instantiate(wiltingIndicator, transform.position, Quaternion.identity);
        }

        Debug.Log("Plant died: " + plantName);
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

    private float CalculateConsumedLight()
    {
        return lightConsumptionRate * Time.deltaTime;
    }

    private float CalculateConsumedNutrient()
    {
        return nutrientConsumptionRate * Time.deltaTime;
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
