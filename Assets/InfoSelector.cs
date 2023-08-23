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

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
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
                    LightSetting selectedLight = jsonLoader.GetLightSettingByName(lightBehavior.lightDataName);
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
