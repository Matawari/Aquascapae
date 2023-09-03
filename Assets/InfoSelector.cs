using UnityEngine;

public class InfoSelector : MonoBehaviour
{
    public JSONLoader jsonLoader;
    public PlantInfoPanel plantInfoPanel;
    public FishInfoPanel fishInfoPanel;
    public LightInfoPanel lightInfoPanel;
    public FilterInfoPanel filterInfoPanel;
    public Camera mainCamera;
    public LayerMask interactableLayerMask;
    public static ObjectPlacementController instance;


    private void Update()
    {

        if (Input.GetMouseButtonDown(0) && !ObjectPlacementController.instance.IsObjectBeingPlaced())
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, float.MaxValue, interactableLayerMask))
            {
                Debug.Log("Hit object: " + hit.transform.gameObject.name);

                PlantBehavior plantBehavior = hit.transform.GetComponent<PlantBehavior>();
                FishBehavior fishBehavior = hit.transform.GetComponent<FishBehavior>();
                LightBehavior lightBehavior = hit.transform.GetComponent<LightBehavior>();
                FilterBehavior filterBehavior = hit.transform.GetComponent<FilterBehavior>();

                if (plantBehavior != null)
                {
                    Debug.Log("Hit object is a plant: " + plantBehavior.plantName);
                    Plant selectedPlant = jsonLoader.GetPlantDataByName(plantBehavior.plantName);
                    plantInfoPanel.gameObject.SetActive(true);
                    fishInfoPanel.gameObject.SetActive(false);
                    lightInfoPanel.gameObject.SetActive(false);
                    filterInfoPanel.gameObject.SetActive(false);
                    plantInfoPanel.UpdatePlantInfo(selectedPlant);
                }
                else if (fishBehavior != null)
                {
                    string fishName = fishBehavior.fish.name;

                    if (string.IsNullOrEmpty(fishName))
                    {
                        Debug.LogError("Fish name is null or empty.");
                        return;
                    }

                    Debug.Log("Hit object is a fish: " + fishName);
                    Fish selectedFish = jsonLoader.GetFishDataByName(fishName);

                    if (selectedFish != null)
                    {
                        fishInfoPanel.gameObject.SetActive(true);
                        plantInfoPanel.gameObject.SetActive(false);
                        lightInfoPanel.gameObject.SetActive(false);
                        filterInfoPanel.gameObject.SetActive(false);
                        fishInfoPanel.UpdateFishInfo(selectedFish);
                    }
                    else
                    {
                        Debug.LogError($"selectedFish is null. Unable to find fish data for '{fishName}'.");
                        foreach (var fish in jsonLoader.fishData.fishes)
                        {
                            Debug.Log("Fish in dataset: " + fish.name);
                        }
                    }
                }
                else if (lightBehavior != null)
                {
                    Debug.Log("Hit object is a light.");
                    JSONLoader.LightSetting selectedLightJSON = jsonLoader.GetLightSettingByName(lightBehavior.lightDataName);

                    LightSetting selectedLight = new LightSetting();
                    selectedLight.name = selectedLightJSON.name;
                    selectedLight.type = selectedLightJSON.type;
                    selectedLight.light_intensity_lux = selectedLightJSON.light_intensity_lux;
                    selectedLight.color_temperature_kelvin = selectedLightJSON.color_temperature_kelvin;
                    selectedLight.intensity_adjustment_factor = selectedLightJSON.intensity_adjustment_factor;
                    selectedLight.price_usd = selectedLightJSON.price_usd;
                    selectedLight.description = selectedLightJSON.description;
                    selectedLight.isOn = selectedLightJSON.isOn;
                    selectedLight.color = selectedLightJSON.color;
                    selectedLight.intensity = selectedLightJSON.intensity;

                    lightInfoPanel.gameObject.SetActive(true);
                    plantInfoPanel.gameObject.SetActive(false);
                    fishInfoPanel.gameObject.SetActive(false);
                    filterInfoPanel.gameObject.SetActive(false);
                    lightInfoPanel.UpdateLightInfo(selectedLight);
                }



                else if (filterBehavior != null)
                {
                    Debug.Log("Hit object is a filter: " + filterBehavior.filterName);
                    Filter selectedFilter = jsonLoader.GetFilterByName(filterBehavior.filterName);
                    filterInfoPanel.gameObject.SetActive(true);
                    plantInfoPanel.gameObject.SetActive(false);
                    fishInfoPanel.gameObject.SetActive(false);
                    lightInfoPanel.gameObject.SetActive(false);
                    filterInfoPanel.UpdateFilterInfo(selectedFilter);
                }
                else
                {
                    Debug.Log("Hit object is not recognized as plant, fish, light, or filter.");
                    plantInfoPanel.gameObject.SetActive(false);
                    fishInfoPanel.gameObject.SetActive(false);
                    lightInfoPanel.gameObject.SetActive(false);
                    filterInfoPanel.gameObject.SetActive(false);
                }
            }
        }
    }
}
