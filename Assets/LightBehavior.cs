using UnityEngine;

public class LightBehavior : MonoBehaviour
{
    public string lightDataName;

    public float CurrentIntensity { get; private set; }

    private JSONLoader jsonLoader;

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
        jsonLoader = FindObjectOfType<JSONLoader>();
        if (jsonLoader == null)
        {
            Debug.LogError("JSONLoader component not found on this GameObject.");
            enabled = false;
        }
    }

    private float GetArtificialLightIntensity()
    {
        if (jsonLoader != null)
        {
            JSONLoader.LightSetting lightSetting = jsonLoader.GetLightSettingByName(lightDataName);
            if (lightSetting != null)
            {
                return lightSetting.intensity;
            }
            else
            {
                Debug.LogError("LightSetting not found in JSONLoader for name: " + lightDataName);
            }
        }
        else
        {
            Debug.LogError("JSONLoader component not found on this GameObject.");
        }

        return 0f; // Return a default value if there's an error
    }
}
