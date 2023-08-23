using UnityEngine;

[System.Serializable]
public class LightData
{
    public LightSetting[] lights;
}

[System.Serializable]
public class LightSetting
{
    public string name;
    public string type;
    public float light_intensity_lux;
    public float color_temperature_kelvin;
    public float intensity_adjustment_factor;
    public float price_usd;
    public string description;
    public bool isOn;
    public Color color; // Custom color property
    public float intensity; // Custom intensity property

    // Add more custom properties as needed
}
