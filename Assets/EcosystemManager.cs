using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using TMPro;
using System.Collections.Generic;

public class EcosystemManager : MonoBehaviour
{
    public float timeStep = 1.0f;

    public WaterBody waterBody;
    public WaterQualityParameters waterQuality;
    public PlantBehaviorManager plantBehavior;
    public List<Substrate> substrates; // Use a list to store multiple substrates
    public PredatorPreyManager predatorPreyManager;
    public LightIntensityManager lightIntensityManager;
    public TemperatureManager temperatureManager;
    public GameObject warningPanel;
    public TextMeshProUGUI warningText;
    public AudioSource warningAudioSource;
    public AudioClip warningSound;
    public Button closeButton;
    public JSONLoader jsonLoader;

    public float maxBacteriaDensityPerArea = 1000.0f; // Maximum bacteria density per square meter

    private float maxBacteriaPopulationThreshold = 5000.0f; // Threshold for high bacterial population warning

    private void Start()
    {
        InitializeComponents();
        warningPanel.SetActive(false);
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(OnCloseButtonClick);
        }
    }

    private void Update()
    {
        SimulateEcosystem();
        SimulateBacterialEvents();
    }

    private void TriggerEvent(string message)
    {
        if (warningText != null)
        {
            warningText.text = message;
            warningPanel.SetActive(true);
            StartCoroutine(DeactivateWarningPanelAfterDelay(5.0f));
        }

        if (warningAudioSource != null && warningSound != null)
        {
            warningAudioSource.clip = warningSound;
            warningAudioSource.Play();
        }
    }

    private IEnumerator DeactivateWarningPanelAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (warningPanel != null)
        {
            warningPanel.SetActive(false);
        }
    }

    private void InitializeComponents()
    {
        if (waterBody == null)
        {
            Debug.LogError("WaterBody component not assigned in the inspector. Please ensure it's manually linked.");
        }

        waterQuality = GetComponent<WaterQualityParameters>();
        if (waterQuality == null)
        {
            Debug.LogError("WaterQualityParameters component not found on the GameObject. Please ensure it's attached.");
        }

        plantBehavior = FindObjectOfType<PlantBehaviorManager>();
        if (plantBehavior == null)
        {
            Debug.LogError("PlantBehaviorManager not found in the scene.");
        }

        predatorPreyManager = FindObjectOfType<PredatorPreyManager>();
        if (predatorPreyManager == null)
        {
            Debug.LogError("PredatorPreyManager not found in the scene.");
        }

        lightIntensityManager = FindObjectOfType<LightIntensityManager>();
        if (lightIntensityManager == null)
        {
            Debug.LogError("LightIntensityManager not found in the scene.");
        }

        temperatureManager = FindObjectOfType<TemperatureManager>();
        if (temperatureManager == null)
        {
            Debug.LogError("TemperatureManager not found in the scene.");
        }

        JSONLoader jsonLoader = FindObjectOfType<JSONLoader>();
        if (jsonLoader != null)
        {
            if (jsonLoader.substrateData != null)
            {
                // Populate the substrates list with the loaded substrates
                substrates = new List<Substrate>(jsonLoader.substrateData.substrates);

                // Check if there are substrates
                if (substrates.Count == 0)
                {
                    Debug.LogError("No substrates found in the loaded JSON data.");
                }
            }
            else
            {
                Debug.LogError("Substrate data not initialized in JSONLoader.");
            }
        }
        else
        {
            Debug.LogError("JSONLoader component not found in the scene.");
        }

        // Calculate the total area of the water body's box collider
        if (waterBody != null)
        {
            Collider waterCollider = waterBody.GetComponent<Collider>();
            if (waterCollider != null)
            {
                float totalArea = waterCollider.bounds.size.x * waterCollider.bounds.size.z;

                // Set the maximum bacteria density based on the total area
                maxBacteriaDensityPerArea = 1000000.0f / totalArea; // 1,000,000 bacteria per square meter
            }
            else
            {
                Debug.LogError("WaterBody Collider not found. Make sure the water body has a Collider component.");
            }
        }
    }


    private void SimulateEcosystem()
    {
        UpdateWaterQuality();
        SimulateNutrientCycling();
        SimulatePlantBehavior();
        SimulatePredatorPreyInteractions();
        SimulateLightIntensity();
        SimulateTemperatureEffects();
        CheckGameEvents();
    }

    private void UpdateWaterQuality()
    {
        if (waterQuality != null && substrates != null && substrates.Count > 0)
        {
            foreach (var substrate in substrates)
            {
                waterQuality.AdjustWaterQualityBasedOnSubstrate(substrate);
            }

            waterQuality.UpdateParameters();
        }
    }

    private void SimulateNutrientCycling()
    {
        if (waterQuality != null && substrates != null && substrates.Count > 0)
        {
            foreach (var substrate in substrates)
            {
                waterQuality.AdjustNutrientLevelsBasedOnSubstrate(substrate);
            }

            waterQuality.UpdateNutrientLevels();
        }
    }

    private void SimulatePlantBehavior()
    {
        float currentTemperatureValue = GetCurrentTemperature();
        if (plantBehavior != null && substrates != null && substrates.Count > 0)
        {
            foreach (var substrate in substrates)
            {
                // Assuming each substrate has a reference to a Plant object
                if (substrate.plant != null)
                {
                    plantBehavior.SimulatePlantGrowth(substrate.plant);
                    plantBehavior.SimulateNutrientUptake(substrate.plant);
                    plantBehavior.SimulateLightSensitivity(substrate.plant);
                }
            }
        }
    }



    public float GetCurrentTemperature()
    {
        if (temperatureManager != null)
        {
            return temperatureManager.GetTemperature();
        }
        else
        {
            return 25.0f; // Default temperature if TemperatureManager is not assigned
        }
    }

    private void SimulatePredatorPreyInteractions()
    {
        if (predatorPreyManager != null)
        {
            predatorPreyManager.SimulateInteractions();
        }
    }

    private void SimulateLightIntensity()
    {
        if (lightIntensityManager != null)
        {
            lightIntensityManager.SimulateLightIntensity();
        }
    }

    private void SimulateTemperatureEffects()
    {
        if (temperatureManager != null)
        {
            temperatureManager.SimulateTemperatureEffects();
        }
    }

    private void CheckGameEvents()
    {
        if (waterQuality != null)
        {
            float algaePopulation = waterQuality.algaePopulation;
            if (algaePopulation >= 1000)
            {
                TriggerEvent("Warning: Algae overgrowth detected!");
            }

            float bacteriaPopulation = waterQuality.bacteriaPopulation;
            if (bacteriaPopulation >= maxBacteriaPopulationThreshold)
            {
                TriggerEvent("Warning: High bacterial population affecting other living things!");
            }

            float ammoniaLevel = waterQuality.GetAmmoniaLevel();
            if (ammoniaLevel >= waterQuality.maxAmmoniaLevel * 0.8f)
            {
                TriggerEvent("Warning: Ammonia pollution detected!");
            }

            float nitrateLevel = waterQuality.GetNitrateLevel();
            if (nitrateLevel >= waterQuality.maxNitrateLevel * 0.8f)
            {
                TriggerEvent("Warning: Nitrate pollution detected!");
            }

            float nitriteLevel = waterQuality.GetNitriteLevel();
            if (nitriteLevel >= waterQuality.maxNitriteLevel * 0.8f)
            {
                TriggerEvent("Warning: Nitrite pollution detected!");
            }

            float pHLevel = waterQuality.GetpH();
            if (pHLevel <= 6.0f || pHLevel >= 8.0f)
            {
                TriggerEvent("Warning: pH level is out of the optimal range!");
            }
        }
    }

    private void SimulateBacterialEvents()
    {
        if (waterQuality != null)
        {
            // Bacteria population is now limited by the maximum population based on water body area
            float currentPopulation = Mathf.Clamp(waterQuality.bacteriaPopulation, 0f, maxBacteriaPopulationThreshold);
            waterQuality.bacteriaPopulation = currentPopulation;
        }
    }

    private void OnCloseButtonClick()
    {
        if (warningPanel != null)
        {
            warningPanel.SetActive(false);
        }
    }
}
