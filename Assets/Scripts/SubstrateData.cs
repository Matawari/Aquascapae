using UnityEngine;
using System;

[System.Serializable]
public class SubstrateData
{
    public Substrate[] substrates;
}

[Serializable]
public class Substrate
{
    public string id;
    public string name;
    public string type;
    public float pH_effect;
    public int cation_exchange_capacity;
    public int nutrient_holding_capacity;
    public float[] particle_size_mm;
    public string color;
    public int price_usd;
    public string description; // Make sure this field is present
    public string suitability_for_plants;
}


[System.Serializable]
public class SubstrateInteraction
{
    public PlantInteraction plants;
    public WaterInteraction water;
}

[System.Serializable]
public class PlantInteraction
{
    public float effectOnGrowthRate;
    public float effectOnNutrientUptake;
}

[System.Serializable]
public class WaterInteraction
{
    public float effectOnpH;
    public float effectOnAmmonia;
    public float effectOnNitrite;
    public float effectOnNitrate;
}
