using UnityEngine;

public class FilterBehavior : MonoBehaviour
{
    public string filterName;
    public JSONLoader.FilterData filterData; // Notice the explicit reference to JSONLoader.FilterData

    private WaterQualityManager waterQualityManager;

    private void Start()
    {
        JSONLoader jsonLoader = FindObjectOfType<JSONLoader>();
        if (jsonLoader != null)
        {
            filterData = jsonLoader.filterData;
            if (filterData == null)
            {
                Debug.LogError("Filter data not found for: " + gameObject.name);
            }
        }
        else
        {
            Debug.LogError("JSONLoader not found in the scene. Make sure it exists.");
        }

        waterQualityManager = FindObjectOfType<WaterQualityManager>();
        if (waterQualityManager == null)
        {
            Debug.LogError("WaterQualityManager not found in the scene. Make sure it exists.");
        }
    }

    public void ApplyFilterEffects()
    {
        ApplyEffectOnpH();
        ApplyEffectOnAmmonia();
        ApplyEffectOnNitrite();
        ApplyEffectOnNitrate();
        ApplyEffectOnOxygen();
    }

    private void ApplyEffectOnpH()
    {
        if (filterData != null)
        {
            float pHChangeRate = filterData.filters[0].pHChangeRate;
            waterQualityManager.SimulatepHChange(-pHChangeRate); // Decrease pH due to filter effect
        }
    }

    private void ApplyEffectOnAmmonia()
    {
        if (filterData != null)
        {
            float ammoniaReduction = filterData.filters[0].ammoniaChangeRate;
            waterQualityManager.SimulateAmmoniaChange(ammoniaReduction);
        }
    }

    private void ApplyEffectOnNitrite()
    {
        if (filterData != null)
        {
            float nitriteChangeRate = filterData.filters[0].nitriteChangeRate;
            waterQualityManager.SimulateNitriteChange(nitriteChangeRate);
        }
    }

    private void ApplyEffectOnNitrate()
    {
        if (filterData != null)
        {
            float nitrateChangeRate = filterData.filters[0].nitrateChangeRate;
            waterQualityManager.SimulateNitrateChange(nitrateChangeRate);
        }
    }

    private void ApplyEffectOnOxygen()
    {
        if (filterData != null)
        {
            float oxygenChangeRate = filterData.filters[0].oxygenChangeRate;
            waterQualityManager.SimulateOxygenChange(oxygenChangeRate);
        }
    }
}
