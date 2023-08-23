using UnityEngine;

public class LightBehavior : MonoBehaviour
{
    public WaterQualityManager waterQualityManager;

    public string lightDataName; // Add this line
    private bool referencesValid = false;

    private void Start()
    {
        InitializeReferences();
        referencesValid = ReferencesAreValid();

        if (referencesValid)
        {
            LoadLightSettingFromJSON();
        }
        else
        {
            enabled = false;
        }
    }

    private void Update()
    {
        if (!referencesValid)
            return;

        float artificialLightIntensity = GetArtificialLightIntensity();
        waterQualityManager.UpdateWaterQualityWithArtificialLight(artificialLightIntensity);
    }

    private void InitializeReferences()
    {
        if (waterQualityManager == null)
        {
            Debug.LogError("WaterQualityManager reference not set in LightBehavior");
            return;
        }
    }

    private void LoadLightSettingFromJSON()
    {
        JSONLoader jsonLoader = FindObjectOfType<JSONLoader>();
        if (jsonLoader != null)
        {
            // Remove the assignment of lightSetting here
        }
        else
        {
            Debug.LogError("JSONLoader component not found on this GameObject.");
        }
    }

    private bool ReferencesAreValid()
    {
        if (waterQualityManager == null)
        {
            Debug.LogError("WaterQualityManager reference not properly set");
            return false;
        }
        return true;
    }

    private float GetArtificialLightIntensity()
    {
        if (!referencesValid)
            return 0f;

        JSONLoader jsonLoader = FindObjectOfType<JSONLoader>();
        if (jsonLoader != null)
        {
            LightSetting lightSetting = jsonLoader.GetLightSettingByName("LED Full Spectrum");
            if (lightSetting != null)
            {
                float baseIntensity = lightSetting.intensity;
                float lightIntensityFactor = waterQualityManager.CalculateLightIntensityFactor();
                float intensityEffect = waterQualityManager.CalculateIntensityEffect();

                float artificialLightIntensity = baseIntensity * lightIntensityFactor * intensityEffect;

                return artificialLightIntensity;
            }
            else
            {
                Debug.LogError("LightSetting not found in JSONLoader");
            }
        }
        else
        {
            Debug.LogError("JSONLoader component not found on this GameObject.");
        }

        return 0f; // Return a default value if there's an error
    }

}
