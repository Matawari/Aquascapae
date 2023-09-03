using UnityEngine;
using System.IO;
using System;
using System.Collections.Generic;

public class JSONLoader : MonoBehaviour
{
    [SerializeField] private string plantFileName = "plants.json";
    [SerializeField] private string filterFileName = "filters.json";
    [SerializeField] private string fishFileName = "fishes.json";
    [SerializeField] private string lightFileName = "lights.json";
    [SerializeField] private string algaeFileName = "algae.json"; // Add this line
    [SerializeField] private string bacteriaFileName = "bacteria.json"; // Add this line
    [SerializeField] private string substrateFileName = "substrates.json";

    public PlantData plantData;
    public FilterData filterData = new FilterData();
    public FishData fishData = new FishData();
    public LightData lightData = new LightData();
    public AlgaeData algaeData = new AlgaeData();
    public BacteriaData bacteriaData = new BacteriaData();
    public SubstrateData substrateData = new SubstrateData();


    public PlantDataPanel plantDataPanel;
    public PlantInfoPanel plantInfoPanel;
    public FilterDataPanel filterDataPanel;
    public FilterInfoPanel filterInfoPanel;
    public FishDataPanel fishDataPanel;
    public FishInfoPanel fishInfoPanel;
    public LightDataPanel lightDataPanel;
    public LightInfoPanel lightInfoPanel;
    public AlgaeDataPanel algaeDataPanel; // Add this reference
    public BacteriaDataPanel bacteriaDataPanel; // Add this reference
    public SubstrateDataPanel substrateDataPanel;
    public SubstrateInteraction substrateInteraction;

    public Dictionary<string, Fish> spawnedFishStats = new Dictionary<string, Fish>();
    public Dictionary<string, Plant> spawnedPlantStats = new Dictionary<string, Plant>();


    public Fish GetSpawnedFishStats(string uniqueName)
    {
        if (spawnedFishStats.ContainsKey(uniqueName))
        {
            return spawnedFishStats[uniqueName];
        }
        return null;
    }

    public Plant GetSpawnedPlantStats(string uniqueName)
    {
        if (spawnedPlantStats.ContainsKey(uniqueName))
        {
            return spawnedPlantStats[uniqueName];
        }
        return null;
    }

    public float GetCurrentTemperature()
    {
        return 25.0f;
    }

    public Fish GetFishDataByName(string fishName)
    {
        Fish fish = Array.Find(fishData.fishes, fish => fish.name == fishName);
        return fish;
    }

    public Filter GetFilterByName(string displayName)
    {
        return Array.Find(filterData.filters, filter => filter.displayName == displayName);
    }

    public Plant GetPlantDataByName(string plantName)
    {
        if (plantData != null && plantData.plants != null)
        {
            return Array.Find(plantData.plants, plant => plant.name == plantName);
        }
        else
        {
            Debug.LogError("Plant data or plantData.plants is null.");
            return null;
        }
    }

    public Algae GetAlgaeDataByName(string algaeName)
    {
        if (algaeData != null && algaeData.algae != null)
        {
            return Array.Find(algaeData.algae, algae => algae.name == algaeName);
        }
        else
        {
            Debug.LogError("Algae data or algaeData.algae is null.");
            return null;
        }
    }

    public Bacteria GetBacteriaDataByName(string bacteriaName)
    {
        if (bacteriaData != null && bacteriaData.bacteria != null)
        {
            return Array.Find(bacteriaData.bacteria, bacteria => bacteria.name == bacteriaName);
        }
        else
        {
            Debug.LogError("Bacteria data or bacteriaData.bacteria is null.");
            return null;
        }
    }

    public LightSetting GetLightSettingByName(string lightDataName)
    {
        Debug.Log("Searching for light setting: " + lightDataName);

        foreach (LightSetting lightSetting in lightData.lights)
        {
            Debug.Log("Checking light setting: " + lightSetting.name);
            if (lightSetting.name == lightDataName)
            {
                return lightSetting;
            }
        }
        Debug.LogError("LightSetting not found in JSONLoader");
        return null;
    }


    public Substrate GetSubstrateDataByName(string substrateName)
    {
        if (substrateData != null && substrateData.substrates != null)
        {
            return Array.Find(substrateData.substrates, substrate => substrate.name == substrateName);
        }
        else
        {
            Debug.LogError("Substrate data or substrateData.substrates is null.");
            return null;
        }
    }


    private void Start()
    {
        FishInfoPanel fishInfoPanel = FindObjectOfType<FishInfoPanel>();
        if (fishInfoPanel != null && fishInfoPanel.gameObject.activeSelf)
        {
            this.fishInfoPanel = fishInfoPanel;

            PlantInfoPanel plantInfoPanel = FindObjectOfType<PlantInfoPanel>();
            if (plantInfoPanel != null)
            {
                plantInfoPanel.jsonLoader = this;
            }

            FilterInfoPanel filterInfoPanel = FindObjectOfType<FilterInfoPanel>();
            if (filterInfoPanel != null)
            {
                filterInfoPanel.SetJSONLoader(this);
            }

            SpawnButtonHandler[] buttonHandlers = FindObjectsOfType<SpawnButtonHandler>();
            foreach (SpawnButtonHandler buttonHandler in buttonHandlers)
            {
                buttonHandler.jsonLoader = this;
            }
        }

        LoadPlantData();
        LoadFilterData();
        LoadFishData();
        LoadLightData();
        LoadSubstrateData();
        LoadAlgaeData(); // Add this line
        LoadBacteriaData(); // Add this line
    }

    public void LoadPlantData()
    {
        string jsonFilePath = Path.Combine(Application.streamingAssetsPath, plantFileName);
        if (File.Exists(jsonFilePath))
        {
            string jsonData = File.ReadAllText(jsonFilePath);
            plantData = JsonUtility.FromJson<PlantData>(jsonData);
        }
        else
        {
            Debug.LogError("JSON file not found: " + jsonFilePath);
        }
    }

    public void LoadFilterData()
    {
        string filterFilePath = Path.Combine(Application.streamingAssetsPath, filterFileName);
        if (File.Exists(filterFilePath))
        {
            try
            {
                string jsonData = File.ReadAllText(filterFilePath);
                filterData = JsonUtility.FromJson<FilterData>(jsonData);

                if (filterData != null && filterData.filters != null && filterData.filters.Length > 0)
                {
                    if (filterDataPanel != null)
                    {
                        filterDataPanel.UpdateFilterData(filterData.filters[0]);
                    }
                    else
                    {
                        Debug.LogError("filterDataPanel is null in JSONLoader");
                    }
                }
                else
                {
                    Debug.LogError("Filter data is null or empty in JSONLoader");
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Error parsing JSON: " + e.Message);
            }
        }
        else
        {
            Debug.LogError("Filter file not found: " + filterFilePath);
        }
    }

    public LightData LoadLightData()
    {
        string lightFilePath = Path.Combine(Application.streamingAssetsPath, lightFileName);
        if (File.Exists(lightFilePath))
        {
            try
            {
                string jsonData = File.ReadAllText(lightFilePath);
                LightData lightData = JsonUtility.FromJson<LightData>(jsonData);
                return lightData;
            }
            catch (Exception e)
            {
                Debug.LogError("Error parsing light JSON: " + e.Message);
            }
        }
        else
        {
            Debug.LogError("Light file not found: " + lightFilePath);
        }

        return null;
    }

    public void LoadFishData()
    {
        string fishFilePath = Path.Combine(Application.streamingAssetsPath, fishFileName);
        if (File.Exists(fishFilePath))
        {
            try
            {
                string jsonData = File.ReadAllText(fishFilePath);
                fishData = JsonUtility.FromJson<FishData>(jsonData);

                if (fishData != null && fishData.fishes != null && fishData.fishes.Length > 0)
                {
                    foreach (Fish loadedFish in fishData.fishes)
                    {
                        Debug.Log("Loaded fish: " + loadedFish.name);
                    }

                    if (fishInfoPanel != null)
                    {
                        if (fishData.fishes.Length > 0)
                        {
                            fishInfoPanel.UpdateFishInfo(fishData.fishes[0]);
                        }
                        else
                        {
                            Debug.LogError("No fish data found in JSONLoader");
                        }
                    }
                    else
                    {
                        Debug.LogError("FishInfoPanel is null in JSONLoader");
                    }
                }
                else
                {
                    Debug.LogError("Fish data is null or empty in JSONLoader");
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Error parsing fish JSON: " + e.Message);
                Debug.LogError("StackTrace: " + e.StackTrace);
            }
        }
        else
        {
            Debug.LogError("Fish file not found: " + fishFilePath);
        }
    }

    public void LoadAlgaeData()
    {
        string algaeFilePath = Path.Combine(Application.streamingAssetsPath, algaeFileName);
        if (File.Exists(algaeFilePath))
        {
            try
            {
                string jsonData = File.ReadAllText(algaeFilePath);
                algaeData = JsonUtility.FromJson<AlgaeData>(jsonData);

                if (algaeData != null)
                {
                    Debug.Log("algaeData is not null");

                    if (algaeData.algae != null)
                    {
                        Debug.Log("algaeData.algae is not null");
                        Debug.Log("Number of algae loaded: " + algaeData.algae.Length);

                        // Existing code
                    }
                    else
                    {
                        Debug.LogError("algaeData.algae is null.");
                    }
                }
                else
                {
                    Debug.LogError("algaeData is null.");
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Error parsing algae JSON: " + e.Message);
                Debug.LogError("StackTrace: " + e.StackTrace);
            }
        }
        else
        {
            Debug.LogError("Algae file not found: " + algaeFilePath);
        }
    }


    public void LoadBacteriaData()
    {
        string bacteriaFilePath = Path.Combine(Application.streamingAssetsPath, bacteriaFileName);
        if (File.Exists(bacteriaFilePath))
        {
            try
            {
                string jsonData = File.ReadAllText(bacteriaFilePath);
                bacteriaData = JsonUtility.FromJson<BacteriaData>(jsonData);

                if (bacteriaData != null)
                {
                    Debug.Log("bacteriaData is not null");

                    if (bacteriaData.bacteria != null)
                    {
                        Debug.Log("bacteriaData.bacteria is not null");
                        Debug.Log("Number of bacteria loaded: " + bacteriaData.bacteria.Length);
                        // Existing code
                    }
                    else
                    {
                        Debug.LogError("bacteriaData.bacteria is null");
                    }
                }
                else
                {
                    Debug.LogError("bacteriaData is null");
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Error parsing bacteria JSON: " + e.Message);
                Debug.LogError("StackTrace: " + e.StackTrace);
            }
        }
        else
        {
            Debug.LogError("Bacteria file not found: " + bacteriaFilePath);
        }
    }


    public void LoadSubstrateData()
    {
        string jsonFilePath = Path.Combine(Application.streamingAssetsPath, substrateFileName);
        Debug.Log("Trying to load substrate data from: " + jsonFilePath);

        if (File.Exists(jsonFilePath))
        {
            try
            {
                string jsonData = File.ReadAllText(jsonFilePath);
                substrateData = JsonUtility.FromJson<SubstrateData>(jsonData);

                if (substrateData != null && substrateData.substrates != null && substrateData.substrates.Length > 0)
                {
                    Debug.Log("Number of substrates loaded: " + substrateData.substrates.Length);  // Added line
                                                                                                   // Update the panel or whatever UI element you have for substrates
                }
                else
                {
                    Debug.LogError("Substrate data is null or empty in JSONLoader");
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Error parsing substrate JSON: " + e.Message);
            }
        }
        else
        {
            Debug.LogError("Substrate JSON file not found: " + jsonFilePath);
        }
    }




    [Serializable]
    public class PlantData
    {
        public Plant[] plants;
    }

    [Serializable]
    public class FilterData
    {
        public Filter[] filters;
    }

    [Serializable]
    public class FishData
    {
        public Fish[] fishes;
    }

    [Serializable]
    public class LightData
    {
        public LightSetting[] lights;
    }

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
    }

    [Serializable]
    public class AlgaeData
    {
        public Algae[] algae;
    }

    [Serializable]
    public class BacteriaData
    {
        public Bacteria[] bacteria;
    }

    [Serializable]
    public class SubstrateData
    {
        public Substrate[] substrates;
    }

}