using UnityEngine;

public class LightBehavior : MonoBehaviour
{
    public string lightDataName;

    public float CurrentIntensity { get; private set; }

    private void Start()
    {
        LoadLightSettingFromJSON();
    }

    private void Update()
    {
        CurrentIntensity = GetArtificialLightIntensity();
    }

    private void LoadLightSettingFromJSON()
    {
        JSONLoader jsonLoader = FindObjectOfType<JSONLoader>();
        if (jsonLoader == null)
        {
            Debug.LogError("JSONLoader component not found on this GameObject.");
            enabled = false;
        }
    }

    private float GetArtificialLightIntensity()
    {
        JSONLoader jsonLoader = FindObjectOfType<JSONLoader>();
        if (jsonLoader != null)
        {
            JSONLoader.LightSetting lightSetting = jsonLoader.GetLightSettingByName("LED Full Spectrum");
            if (lightSetting != null)
            {
                return lightSetting.intensity;
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
