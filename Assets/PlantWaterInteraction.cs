using UnityEngine;

public class PlantWaterInteraction : MonoBehaviour
{
    public WaterQualityManager waterQualityManager;
    public PlantBehavior plantBehavior;
    public JSONLoader jsonLoader;
    public PlantInfoPanel plantInfoPanel;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            if (plantBehavior == null)
            {
                Debug.LogError("PlantBehavior not found.");
                return;
            }

            if (plantBehavior.plantData == null)
                return;

            float lightIntensityFactor = CalculateLightIntensityFactor();
            float currentTemperature = jsonLoader.GetCurrentTemperature();

            if (waterQualityManager != null)
            {
                waterQualityManager.ApplyPlantEffect(plantBehavior.plantData, lightIntensityFactor, currentTemperature);
            }
            else
            {
                Debug.LogError("WaterQualityManager not found.");
            }

            if (plantInfoPanel != null)
                plantInfoPanel.UpdatePlantInfo(plantBehavior.plantData);
        }
    }

    private float CalculateLightIntensityFactor()
    {
        float totalIntensityFactor = 0f;

        if (waterQualityManager != null)
        {
            if (plantBehavior == null)
            {
                Debug.LogError("PlantBehavior not found.");
                return 0f;
            }

            LightData lightData = waterQualityManager.GetLightData();

            if (lightData != null && lightData.lights != null)
            {
                LightSetting[] lightSettings = lightData.lights;

                foreach (LightSetting settings in lightSettings)
                {
                    totalIntensityFactor += settings.intensity_adjustment_factor;
                }
            }
            else
            {
                Debug.LogWarning("LightData or lightData.lights is null.");
            }
        }
        else
        {
            Debug.LogError("WaterQualityManager not found.");
            return 0f;
        }

        return totalIntensityFactor;
    }
}
