using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LightInfoPanel : MonoBehaviour
{
    public TextMeshProUGUI nameLabel;
    public TextMeshProUGUI typeText;
    public Slider intensitySlider;
    public Slider temperatureSlider;
    public Slider adjustmentSlider;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI intensityText;
    public TextMeshProUGUI temperatureText;
    public Toggle toggleToggle;
    public Button closeButton;
    public Light lightGameObject;

    private LightSetting currentLightSetting;
    private float initialIntensityLux;
    private float initialColorTemperatureKelvin;

    private const string IntensitySliderKey = "IntensitySlider";
    private const string TemperatureSliderKey = "TemperatureSlider";
    private const string AdjustmentSliderKey = "AdjustmentSlider";
    private const string LightStateKey = "LightState";

    private bool isInitialized = false;

    private void Start()
    {
        closeButton.onClick.AddListener(ClosePanel);
        toggleToggle.onValueChanged.AddListener(ToggleLight);
        intensitySlider.onValueChanged.AddListener(ChangeIntensity);
        temperatureSlider.onValueChanged.AddListener(ChangeColorTemperature);
        adjustmentSlider.onValueChanged.AddListener(ChangeAdjustment);

        if (Application.isPlaying)
        {
            InitializeLightComponent();
        }
    }

    public void UpdateLightInfo(LightSetting lightSetting)
    {
        if (lightSetting != null && lightGameObject != null)
        {
            UpdateLightIntensity(lightSetting.light_intensity_lux);
            UpdateColorTemperature(lightSetting.color_temperature_kelvin);
            UpdateLightState(lightSetting.isOn);
            nameLabel.text = lightSetting.name;
            typeText.text = "Type: " + lightSetting.type;
            intensitySlider.maxValue = initialIntensityLux = lightSetting.light_intensity_lux;
            temperatureSlider.maxValue = initialColorTemperatureKelvin = lightSetting.color_temperature_kelvin;
            intensitySlider.value = GetSavedSliderValue(IntensitySliderKey, initialIntensityLux);
            temperatureSlider.value = GetSavedSliderValue(TemperatureSliderKey, initialColorTemperatureKelvin);
            adjustmentSlider.value = GetSavedSliderValue(AdjustmentSliderKey, lightSetting.intensity_adjustment_factor);
            descriptionText.text = "Description: " + lightSetting.description;
            toggleToggle.isOn = GetSavedLightState();
            currentLightSetting = lightSetting;
        }
        else
        {
            Debug.LogError("LightSetting or lightGameObject is null in UpdateLightInfo.");
        }
    }

    private void UpdateColorTemperature(float kelvinTemperature)
    {
        if (lightGameObject != null)
        {
            lightGameObject.colorTemperature = kelvinTemperature;
            temperatureText.text = kelvinTemperature.ToString("F1") + " (K)";
        }
    }

    private void ChangeColorTemperature(float kelvinTemperature)
    {
        if (lightGameObject != null)
        {
            UpdateColorTemperature(kelvinTemperature);
        }
    }

    private void UpdateLightIntensity(float luxIntensity)
    {
        if (lightGameObject != null)
        {
            lightGameObject.intensity = luxIntensity;
            intensityText.text = luxIntensity.ToString("F1") + " (lux)";
        }
    }

    private void ChangeIntensity(float luxIntensity)
    {
        if (lightGameObject != null)
        {
            UpdateLightIntensity(luxIntensity);
        }
    }

    private void UpdateLightState(bool isOn)
    {
        if (lightGameObject != null)
        {
            lightGameObject.enabled = isOn;
        }
    }

    private void ToggleLight(bool isOn)
    {
        UpdateLightState(isOn);
    }

    private void ClosePanel()
    {
        SaveSliderPositions();
        SaveLightState(toggleToggle.isOn);
        gameObject.SetActive(false);
    }

    private void SaveSliderPositions()
    {
        PlayerPrefs.SetFloat(IntensitySliderKey, intensitySlider.value);
        PlayerPrefs.SetFloat(TemperatureSliderKey, temperatureSlider.value);
        PlayerPrefs.SetFloat(AdjustmentSliderKey, adjustmentSlider.value);
        PlayerPrefs.Save();
    }

    private float GetSavedSliderValue(string key, float defaultValue)
    {
        return PlayerPrefs.HasKey(key) ? PlayerPrefs.GetFloat(key) : defaultValue;
    }

    private void SaveLightState(bool isOn)
    {
        PlayerPrefs.SetInt(LightStateKey, isOn ? 1 : 0);
    }

    private bool GetSavedLightState()
    {
        return PlayerPrefs.HasKey(LightStateKey) ? PlayerPrefs.GetInt(LightStateKey) == 1 : true;
    }

    private void InitializeLightComponent()
    {
        if (!isInitialized)
        {
            UpdateLightIntensity(initialIntensityLux);
            UpdateColorTemperature(initialColorTemperatureKelvin);
            UpdateLightState(toggleToggle.isOn);

            isInitialized = true;
        }
    }

    private void ChangeAdjustment(float value)
    {
        // Implement your adjustment logic here
    }
}
