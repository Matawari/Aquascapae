using UnityEngine;

public class FishWaterInteraction : MonoBehaviour
{
    public WaterQualityManager waterQualityManager;
    public JSONLoader jsonLoader;
    public FishInfoPanel fishInfoPanel; // Reference to FishInfoPanel

    private FishBehavior fishBehavior;
    private bool isCollidingWithWater = false;
    private LightSetting[] lightSettings;

    private void Start()
    {
        fishBehavior = GetComponent<FishBehavior>();

        if (waterQualityManager == null)
        {
            Debug.LogError("WaterQualityManager reference is not set.");
        }

        if (jsonLoader == null)
        {
            Debug.LogError("JSONLoader reference is not set.");
        }
        else
        {
            lightSettings = jsonLoader.lightData.lights;
        }
    }

    private void Update()
    {
        if (fishBehavior == null)
        {
            Debug.LogError("FishBehavior not found.");
            return;
        }

        FishData fishData = fishBehavior.fishData; // Changed type here

        if (fishData.fishes[0].health <= 0 || fishData.fishes[0].stress >= 100) return;

        if (isCollidingWithWater && waterQualityManager != null && jsonLoader != null)
        {
            float pHValue = waterQualityManager.GetpH();
            float ammoniaValue = waterQualityManager.GetAmmoniaLevel();
            float nitriteValue = waterQualityManager.GetNitriteLevel();
            float nitrateValue = waterQualityManager.GetNitrateLevel();
            float o2ProductionRate = waterQualityManager.GetOxygenProduction();
            float co2AbsorptionRate = waterQualityManager.GetCO2AbsorptionRate();
            float currentTemperature = jsonLoader.GetCurrentTemperature();
            float lightIntensityFactor = CalculateLightIntensityFactor();

            Fish fish = fishData.fishes[0]; // Retrieve the specific Fish object

            fishBehavior.ApplyWaterEffects(fishData, pHValue, ammoniaValue, nitriteValue, nitrateValue, o2ProductionRate, co2AbsorptionRate, currentTemperature);

            foreach (Fish fishInstance in waterQualityManager.FishInstances)
            {
                waterQualityManager.ApplyFishEffect(fishInstance, lightIntensityFactor);
            }

            if (fishInfoPanel != null)
            {
                // Update the FishInfoPanel here with relevant information
                fishInfoPanel.UpdateFishInfo(fish);
            }
        }
    }


    private float CalculateLightIntensityFactor()
    {
        float totalIntensityFactor = 0f;

        foreach (LightSetting lightSetting in lightSettings)
        {
            float intensityFactor = lightSetting.intensity_adjustment_factor;
            totalIntensityFactor += intensityFactor;
        }

        return totalIntensityFactor;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            isCollidingWithWater = true;
            if (waterQualityManager != null && fishBehavior != null)
            {
                float lightIntensityFactor = CalculateLightIntensityFactor();
                waterQualityManager.ApplyFishEffect(fishBehavior.fishData.fishes[0], lightIntensityFactor);
            }
            Debug.Log("Fish entered the water.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            isCollidingWithWater = false;
            if (waterQualityManager != null && fishBehavior != null)
            {
                float lightIntensityFactor = CalculateLightIntensityFactor();
                waterQualityManager.RemoveFishEffect(fishBehavior.fishData.fishes[0], lightIntensityFactor);
            }
            Debug.Log("Fish exited the water.");
        }
    }
}
