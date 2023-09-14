using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public class LightInfoPanel : MonoBehaviour
{
    public Light lightGameObject;
    public JSONLoader jsonLoader;

    public TMP_Text autoOnHour;
    public TMP_Text autoOnMinute;
    public TMP_Text autoOffHour;
    public TMP_Text autoOffMinute;

    public Button autoOnLeftArrow;
    public Button autoOnRightArrow;
    public Button autoOnUpArrow;
    public Button autoOnDownArrow;

    public Button autoOffLeftArrow;
    public Button autoOffRightArrow;
    public Button autoOffUpArrow;
    public Button autoOffDownArrow;

    public Slider intensitySlider;
    public TextMeshProUGUI intensityText;
    public Button intensityLeftButton;
    public Button intensityRightButton;

    public Slider temperatureSlider;
    public TextMeshProUGUI temperatureText;
    public Button temperatureLeftButton;
    public Button temperatureRightButton;

    public Button powerButton;

    private JSONLoader.Lights currentLights;
    private TMP_Text selectedAutoOnField;
    private TMP_Text selectedAutoOffField;

    private bool isAdjustingIntensity = false;
    private bool isAdjustingTemperature = false;
    private int intensityAdjustmentDirection = 0;
    private int temperatureAdjustmentDirection = 0;
    private const float FAST_ADJUSTMENT_SPEED = 0.02f;

    private void Start()
    {
        powerButton.onClick.AddListener(ToggleLightPower);

        autoOnLeftArrow.onClick.AddListener(() => SwitchSelectedField(ref selectedAutoOnField, autoOnHour, autoOnMinute, true));
        autoOnRightArrow.onClick.AddListener(() => SwitchSelectedField(ref selectedAutoOnField, autoOnHour, autoOnMinute, false));
        autoOnUpArrow.onClick.AddListener(() => AdjustTime(selectedAutoOnField, 1));
        autoOnDownArrow.onClick.AddListener(() => AdjustTime(selectedAutoOnField, -1));

        autoOffLeftArrow.onClick.AddListener(() => SwitchSelectedField(ref selectedAutoOffField, autoOffHour, autoOffMinute, true));
        autoOffRightArrow.onClick.AddListener(() => SwitchSelectedField(ref selectedAutoOffField, autoOffHour, autoOffMinute, false));
        autoOffUpArrow.onClick.AddListener(() => AdjustTime(selectedAutoOffField, 1));
        autoOffDownArrow.onClick.AddListener(() => AdjustTime(selectedAutoOffField, -1));

        intensityLeftButton.onClick.AddListener(() => StartAdjustingSlider(intensitySlider, -1));
        intensityRightButton.onClick.AddListener(() => StartAdjustingSlider(intensitySlider, 1));

        temperatureLeftButton.onClick.AddListener(() => StartAdjustingSlider(temperatureSlider, -1));
        temperatureRightButton.onClick.AddListener(() => StartAdjustingSlider(temperatureSlider, 1));

        intensitySlider.onValueChanged.AddListener(UpdateIntensityText);
        temperatureSlider.onValueChanged.AddListener(UpdateTemperatureText);

        AddPointerUpListener(intensityLeftButton, StopAdjustingSlider);
        AddPointerUpListener(intensityRightButton, StopAdjustingSlider);
        AddPointerUpListener(temperatureLeftButton, StopAdjustingSlider);
        AddPointerUpListener(temperatureRightButton, StopAdjustingSlider);

        selectedAutoOnField = autoOnHour;
        selectedAutoOffField = autoOffHour;
        StartBlinking(selectedAutoOnField);
        StartBlinking(selectedAutoOffField);

        intensitySlider.value = intensitySlider.maxValue;
        temperatureSlider.value = temperatureSlider.maxValue;
    }

    private void AddPointerUpListener(Button button, UnityEngine.Events.UnityAction callback)
    {
        EventTrigger trigger = button.gameObject.AddComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerUp;
        entry.callback.AddListener((eventData) => callback());
        trigger.triggers.Add(entry);
    }

    private void AdjustTime(TMP_Text field, int adjustment)
    {
        int value = int.Parse(field.text);
        if (field == autoOnHour || field == autoOffHour)
        {
            value = Mathf.Clamp(value + adjustment, 0, 23);
        }
        else
        {
            value += adjustment;
            if (value >= 60)
            {
                value -= 60;
            }
            else if (value < 0)
            {
                value += 60;
            }
        }
        field.text = value.ToString("00");
    }

    private void SwitchSelectedField(ref TMP_Text currentField, TMP_Text hourField, TMP_Text minuteField, bool isLeft)
    {
        StopBlinking(currentField);
        currentField = currentField == hourField ? minuteField : hourField;
        StartBlinking(currentField);
        if (isLeft)
        {
            // Add logic for blinking once when left button is pressed
        }
    }

    private void StartBlinking(TMP_Text field)
    {
        field.enableVertexGradient = true;
        field.colorGradientPreset = null;
        field.enableAutoSizing = true;
    }

    private void StopBlinking(TMP_Text field)
    {
        field.enableVertexGradient = false;
        field.enableAutoSizing = false;
    }

    private void ToggleLightPower()
    {
        lightGameObject.enabled = !lightGameObject.enabled;
    }



    private void StartAdjustingSlider(Slider slider, int direction)
    {
        if (slider == intensitySlider)
        {
            isAdjustingIntensity = true;
            intensityAdjustmentDirection = direction;
            StartCoroutine(AdjustIntensity());
        }
        else if (slider == temperatureSlider)
        {
            isAdjustingTemperature = true;
            temperatureAdjustmentDirection = direction;
            StartCoroutine(AdjustTemperature());
        }
    }

    private void StopAdjustingSlider()
    {
        isAdjustingIntensity = false;
        isAdjustingTemperature = false;
    }

    private IEnumerator AdjustIntensity()
    {
        while (isAdjustingIntensity)
        {
            AdjustSlider(intensitySlider, intensityAdjustmentDirection);
            yield return new WaitForSeconds(FAST_ADJUSTMENT_SPEED);
        }
    }

    private IEnumerator AdjustTemperature()
    {
        while (isAdjustingTemperature)
        {
            AdjustSlider(temperatureSlider, temperatureAdjustmentDirection);
            yield return new WaitForSeconds(FAST_ADJUSTMENT_SPEED);
        }
    }

    private void AdjustSlider(Slider slider, int adjustment)
    {
        float newValue = slider.value + adjustment;
        if (newValue > slider.maxValue)
        {
            slider.value = slider.maxValue;
        }
        else if (newValue < 0)
        {
            slider.value = 0;
        }
        else
        {
            slider.value = newValue;
        }
    }

    private void UpdateIntensityText(float value)
    {
        intensityText.text = Mathf.RoundToInt(value).ToString();
        lightGameObject.intensity = value / intensitySlider.maxValue;
    }

    private void UpdateTemperatureText(float value)
    {
        temperatureText.text = Mathf.RoundToInt(value).ToString();
        lightGameObject.color = KelvinToRGB(value);
    }

    public void UpdateLightInfo(JSONLoader.Lights lightSetting)
    {
        if (lightSetting != null && lightGameObject != null)
        {
            intensitySlider.maxValue = lightSetting.light_intensity_lux;
            temperatureSlider.maxValue = lightSetting.color_temperature_kelvin;

            intensitySlider.value = intensitySlider.maxValue;
            temperatureSlider.value = temperatureSlider.maxValue;

            UpdateIntensityText(intensitySlider.value);
            UpdateTemperatureText(temperatureSlider.value);

            ToggleLight(lightSetting.isOn);
            currentLights = lightSetting;
        }
        else
        {
            Debug.LogError("Lights or lightGameObject is null in UpdateLightInfo.");
        }
    }

    private Color KelvinToRGB(float kelvin)
    {
        float temp = kelvin / 100;
        float red, green, blue;

        if (temp <= 66)
        {
            red = 255;
            green = temp;
            green = 99.4708025861f * Mathf.Log(green) - 161.1195681661f;
            if (temp <= 19)
            {
                blue = 0;
            }
            else
            {
                blue = temp - 10;
                blue = 138.5177312231f * Mathf.Log(blue) - 305.0447927307f;
            }
        }
        else
        {
            red = temp - 60;
            red = 329.698727446f * Mathf.Pow(red, -0.1332047592f);
            green = temp - 60;
            green = 288.1221695283f * Mathf.Pow(green, -0.0755148492f);
            blue = 255;
        }

        return new Color(red / 255.0f, green / 255.0f, blue / 255.0f);
    }

    private void ToggleLight(bool isOn)
    {
        lightGameObject.enabled = isOn;
    }
}
