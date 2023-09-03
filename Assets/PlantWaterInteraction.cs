using UnityEngine;

public class PlantWaterInteraction : MonoBehaviour
{
    private JSONLoader jsonLoader;
    public PlantBehavior plantBehavior;
    public PlantInfoPanel plantInfoPanel;

    private void Start()
    {
        jsonLoader = GetComponent<JSONLoader>();

        if (jsonLoader == null)
        {
            jsonLoader = FindObjectOfType<JSONLoader>();

            if (jsonLoader == null)
            {
                Debug.LogError("JSONLoader component not found on GameObject: " + gameObject.name);
                return;
            }
        }
    }



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

            // Apply water effects or interactions here using plantBehavior and lightIntensityFactor

            if (plantInfoPanel != null)
                plantInfoPanel.UpdatePlantInfo(plantBehavior.plantData);
        }
    }

    private float CalculateLightIntensityFactor()
    {
        float totalIntensityFactor = 0f;

        if (jsonLoader != null)
        {
            JSONLoader.LightData lightData = jsonLoader.LoadLightData();

            if (lightData != null && lightData.lights != null)
            {
                JSONLoader.LightSetting[] lightSettings = lightData.lights;

                foreach (JSONLoader.LightSetting settings in lightSettings)
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
            Debug.LogError("JSONLoader not found.");
            return 0f;
        }

        return totalIntensityFactor;
    }

}
