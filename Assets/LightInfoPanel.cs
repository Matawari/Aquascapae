using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public class LightInfoPanel : MonoBehaviour
{
    [SerializeField]
    private Light lightGameObject;
    [SerializeField]
    private JSONLoader jsonLoader;
    [SerializeField]
    private AudioClip clickSound;
    private AudioSource audioSource;

    [SerializeField]
    private TMP_Text autoOnHour, autoOnMinute, autoOffHour, autoOffMinute;

    [SerializeField]
    private Button autoOnLeftArrow, autoOnRightArrow, autoOnUpArrow, autoOnDownArrow;
    [SerializeField]
    private Button autoOffLeftArrow, autoOffRightArrow, autoOffUpArrow, autoOffDownArrow;
    [SerializeField]
    private Slider intensitySlider;
    [SerializeField]
    private TextMeshProUGUI intensityText;
    [SerializeField]
    private Button intensityLeftButton, intensityRightButton;
    [SerializeField]
    private Slider temperatureSlider;
    [SerializeField]
    private TextMeshProUGUI temperatureText;
    [SerializeField]
    private Button temperatureLeftButton, temperatureRightButton;
    [SerializeField]
    private Button powerButton;
    private bool isAdjusting = false;


    private JSONLoader.Lights currentLights;
    private TMP_Text selectedAutoOnField, selectedAutoOffField;
    private bool isAdjustingIntensity = false, isAdjustingTemperature = false;
    private const float FAST_ADJUSTMENT_SPEED = 0.02f;
    private Coroutine currentBlinkingCoroutine;

    private void Awake()
    {
        if (!audioSource)
            audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = clickSound;
    }

    private void Start()
    {
        SetUpListeners();
        selectedAutoOnField = autoOnHour;
        selectedAutoOffField = autoOffHour;
        StartBlinking(selectedAutoOnField);
        StartBlinking(selectedAutoOffField);

        LoadSettings();
    }

    private void SetUpListeners()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = clickSound;

        powerButton.onClick.AddListener(() => { ToggleLightPower(); PlayClickSound(); });

        autoOnLeftArrow.onClick.AddListener(() => { PlayClickSound(); SwitchSelectedField(ref selectedAutoOnField, autoOnHour, autoOnMinute); });
        autoOnRightArrow.onClick.AddListener(() => { PlayClickSound(); SwitchSelectedField(ref selectedAutoOnField, autoOnHour, autoOnMinute); });
        autoOnUpArrow.onClick.AddListener(() => { StopAllCoroutines(); PlayClickSound(); AdjustTime(selectedAutoOnField, 1); });
        autoOnDownArrow.onClick.AddListener(() => { StopAllCoroutines(); PlayClickSound(); AdjustTime(selectedAutoOnField, -1); });

        autoOffLeftArrow.onClick.AddListener(() => { PlayClickSound(); SwitchSelectedField(ref selectedAutoOffField, autoOffHour, autoOffMinute); });
        autoOffRightArrow.onClick.AddListener(() => { PlayClickSound(); SwitchSelectedField(ref selectedAutoOffField, autoOffHour, autoOffMinute); });
        autoOffUpArrow.onClick.AddListener(() => { StopAllCoroutines(); PlayClickSound(); AdjustTime(selectedAutoOffField, 1); });
        autoOffDownArrow.onClick.AddListener(() => { StopAllCoroutines(); PlayClickSound(); AdjustTime(selectedAutoOffField, -1); });

        intensitySlider.onValueChanged.AddListener(UpdateIntensityText);
        temperatureSlider.onValueChanged.AddListener(UpdateTemperatureText);

        AddPointerUpListener(intensityLeftButton, StopAdjustingSlider);
        AddPointerUpListener(intensityRightButton, StopAdjustingSlider);
        AddPointerUpListener(temperatureLeftButton, StopAdjustingSlider);
        AddPointerUpListener(temperatureRightButton, StopAdjustingSlider);

        intensityLeftButton.onClick.AddListener(() => { StopAllCoroutines(); PlayClickSound(); AdjustSlider(intensitySlider, -1); });
        intensityRightButton.onClick.AddListener(() => { StopAllCoroutines(); PlayClickSound(); AdjustSlider(intensitySlider, 1); });
        temperatureLeftButton.onClick.AddListener(() => { StopAllCoroutines(); PlayClickSound(); AdjustSlider(temperatureSlider, -1); });
        temperatureRightButton.onClick.AddListener(() => { StopAllCoroutines(); PlayClickSound(); AdjustSlider(temperatureSlider, 1); });

        AddPointerDownListener(intensityLeftButton, () => StartCoroutine(ContinuousAdjustSlider(intensitySlider, -1)));
        AddPointerDownListener(intensityRightButton, () => StartCoroutine(ContinuousAdjustSlider(intensitySlider, 1)));
        AddPointerDownListener(temperatureLeftButton, () => StartCoroutine(ContinuousAdjustSlider(temperatureSlider, -1)));
        AddPointerDownListener(temperatureRightButton, () => StartCoroutine(ContinuousAdjustSlider(temperatureSlider, 1)));

        AddPointerDownListener(autoOnLeftArrow, () => StartCoroutine(ContinuousSwitchField(selectedAutoOnField, autoOnHour, autoOnMinute)));
        AddPointerUpListener(autoOnLeftArrow, StopAllCoroutines);
        AddPointerDownListener(autoOnRightArrow, () => StartCoroutine(ContinuousSwitchField(selectedAutoOnField, autoOnHour, autoOnMinute)));
        AddPointerDownListener(autoOnUpArrow, () => StartCoroutine(ContinuousAdjustTime(selectedAutoOnField, 1)));
        AddPointerDownListener(autoOnDownArrow, () => StartCoroutine(ContinuousAdjustTime(selectedAutoOnField, -1)));

        AddPointerDownListener(autoOffLeftArrow, () => StartCoroutine(ContinuousSwitchField(selectedAutoOffField, autoOffHour, autoOffMinute)));
        AddPointerDownListener(autoOffRightArrow, () => StartCoroutine(ContinuousSwitchField(selectedAutoOffField, autoOffHour, autoOffMinute)));
        AddPointerDownListener(autoOffUpArrow, () => StartCoroutine(ContinuousAdjustTime(selectedAutoOffField, 1)));
        AddPointerDownListener(autoOffDownArrow, () => StartCoroutine(ContinuousAdjustTime(selectedAutoOffField, -1)));

        selectedAutoOnField = autoOnHour;
        selectedAutoOffField = autoOffHour;
        StartBlinking(selectedAutoOnField);
        StartBlinking(selectedAutoOffField);

        intensitySlider.value = intensitySlider.maxValue;
        temperatureSlider.value = temperatureSlider.maxValue;

        LoadSettings();
    }



    public Light LightGameObject
    {
        get { return lightGameObject; }
        set { lightGameObject = value; }
    }

    public Slider IntensitySlider
    {
        get { return intensitySlider; }
        set { intensitySlider = value; }
    }

    public Slider TemperatureSlider
    {
        get { return temperatureSlider; }
        set { temperatureSlider = value; }
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SaveSettings();
            gameObject.SetActive(false);
        }
    }

    private void SaveSettings()
    {
        PlayerPrefs.SetFloat("Intensity", intensitySlider.value);
        PlayerPrefs.SetFloat("Temperature", temperatureSlider.value);
        PlayerPrefs.SetInt("AutoOnHour", int.Parse(autoOnHour.text));
        PlayerPrefs.SetInt("AutoOnMinute", int.Parse(autoOnMinute.text));
        PlayerPrefs.SetInt("AutoOffHour", int.Parse(autoOffHour.text));
        PlayerPrefs.SetInt("AutoOffMinute", int.Parse(autoOffMinute.text));
        PlayerPrefs.SetInt("IsLightOn", lightGameObject.enabled ? 1 : 0);

        PlayerPrefs.Save();
    }

    private void PlayClickSound()
    {
        audioSource.Play();
    }

    private void AddPointerDownListener(Button button, UnityEngine.Events.UnityAction callback)
    {
        EventTrigger trigger = button.gameObject.GetComponent<EventTrigger>() ?? button.gameObject.AddComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerDown;
        entry.callback.AddListener((eventData) => callback());
        trigger.triggers.Add(entry);
    }

    private void AddPointerUpListener(Button button, UnityEngine.Events.UnityAction callback)
    {
        EventTrigger trigger = button.gameObject.GetComponent<EventTrigger>() ?? button.gameObject.AddComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerUp;
        entry.callback.AddListener((eventData) => callback());
        trigger.triggers.Add(entry);
    }

    private IEnumerator ContinuousSwitchField(TMP_Text currentField, TMP_Text hourField, TMP_Text minuteField)
    {
        while (true)
        {
            SwitchSelectedField(ref currentField, hourField, minuteField);
            yield return new WaitForSeconds(FAST_ADJUSTMENT_SPEED);
        }
    }

    private IEnumerator ContinuousAdjustTime(TMP_Text field, int adjustment)
    {
        while (true)
        {
            AdjustTime(field, adjustment);
            yield return new WaitForSeconds(FAST_ADJUSTMENT_SPEED);
        }
    }

    private IEnumerator ContinuousAdjustSlider(Slider slider, int direction)
    {
        while (isAdjusting)
        {
            AdjustSlider(slider, direction);
            yield return new WaitForSeconds(FAST_ADJUSTMENT_SPEED);
        }
    }


    private void StartAdjustingSlider(Slider slider, int direction)
    {
        isAdjusting = true;
    }

    private void StopAdjustingSlider()
    {
        isAdjusting = false;
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

    private void SwitchSelectedField(ref TMP_Text currentField, TMP_Text hourField, TMP_Text minuteField)
    {
        StopBlinking();
        currentField = currentField == hourField ? minuteField : hourField;
        StartBlinking(currentField);
    }


    private IEnumerator BlinkText(TMP_Text field)
    {
        while (true)
        {
            field.alpha = 0.5f; // Half transparent
            yield return new WaitForSeconds(0.5f); // Wait for 0.5 seconds
            field.alpha = 1.0f; // Fully opaque
            yield return new WaitForSeconds(0.5f); // Wait for 0.5 seconds
        }
    }

    private void StartBlinking(TMP_Text field)
    {
        if (currentBlinkingCoroutine != null)
        {
            StopCoroutine(currentBlinkingCoroutine);
        }
        currentBlinkingCoroutine = StartCoroutine(BlinkText(field));
    }

    private void StopBlinking()
    {
        if (currentBlinkingCoroutine != null)
        {
            StopCoroutine(currentBlinkingCoroutine);
            currentBlinkingCoroutine = null;
        }
    }

    private void ToggleLightPower()
    {
        lightGameObject.enabled = !lightGameObject.enabled;
    }

    private void AdjustSlider(Slider slider, int adjustment)
    {
        float newValue = slider.value + adjustment;
        if (newValue > slider.maxValue)
        {
            slider.value = slider.maxValue;
        }
        else if (newValue < slider.minValue)
        {
            slider.value = slider.minValue;
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

    private void ToggleLight(bool isOn)
    {
        if (lightGameObject != null)
        {
            lightGameObject.enabled = isOn;
        }
        else
        {
            Debug.LogError("lightGameObject is null in ToggleLight.");
        }
    }

    private void LoadSettings()
    {
        if (PlayerPrefs.HasKey("Intensity"))
        {
            intensitySlider.value = PlayerPrefs.GetFloat("Intensity");
            UpdateIntensityText(intensitySlider.value);
        }

        if (PlayerPrefs.HasKey("Temperature"))
        {
            temperatureSlider.value = PlayerPrefs.GetFloat("Temperature");
            UpdateTemperatureText(temperatureSlider.value);
        }

        if (PlayerPrefs.HasKey("AutoOnHour"))
        {
            autoOnHour.text = PlayerPrefs.GetInt("AutoOnHour").ToString("00");
        }

        if (PlayerPrefs.HasKey("AutoOnMinute"))
        {
            autoOnMinute.text = PlayerPrefs.GetInt("AutoOnMinute").ToString("00");
        }

        if (PlayerPrefs.HasKey("AutoOffHour"))
        {
            autoOffHour.text = PlayerPrefs.GetInt("AutoOffHour").ToString("00");
        }

        if (PlayerPrefs.HasKey("AutoOffMinute"))
        {
            autoOffMinute.text = PlayerPrefs.GetInt("AutoOffMinute").ToString("00");
        }

        if (PlayerPrefs.HasKey("IsLightOn"))
        {
            lightGameObject.enabled = PlayerPrefs.GetInt("IsLightOn") == 1;
        }
    }
}
