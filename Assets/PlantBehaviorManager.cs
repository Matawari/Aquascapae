using System.Collections.Generic;
using UnityEngine;

namespace AquascapeMadness
{
    public class PlantBehaviorManager : MonoBehaviour
    {
        public WaterQualityParameters waterQuality;
        public JSONLoader jsonLoader;

        private List<Plant> allPlants = new List<Plant>();

        private void Start()
        {
            JSONLoader jsonLoader = FindObjectOfType<JSONLoader>(); // Find the JSONLoader component in the scene
            if (jsonLoader != null)
            {
                allPlants = new List<Plant>(jsonLoader.plantData.plants); // Load plant data from JSONLoader
            }
            else
            {
                Debug.LogError("JSONLoader component not found.");
            }
        }


        public void SimulatePlantBehavior()
        {
            foreach (Plant plant in allPlants)
            {
                SimulatePlantGrowth(plant);
                SimulateNutrientUptake(plant);
                SimulateLightSensitivity(plant);
                // You can add more behaviors and interactions here
            }
        }

        private void SimulatePlantGrowth(Plant plant)
        {
            float growthRate = plant.CalculateAdjustedGrowthRate(GetCurrentTemperature());
            plant.Size += growthRate * Time.deltaTime;
        }

        private void SimulateNutrientUptake(Plant plant)
        {
            float nutrientUptakeRate = CalculateNutrientUptakeRate(plant);
            waterQuality.ReduceNutrientLevels(nutrientUptakeRate * Time.deltaTime);
        }

        private void SimulateLightSensitivity(Plant plant)
        {
            float lightIntensity = CalculateLightIntensity(plant.Position); // Use plant.Position here

            if (lightIntensity > plant.LightThreshold)
            {
                // Perform actions for sufficient light
            }
            else
            {
                // Adjust behavior for low light
            }
        }


        private float CalculateGrowthRate(Plant plant)
        {
            // Calculate growth rate based on nutrient levels, temperature, etc.
            // For now, let's return a placeholder value
            return 0.1f; // Adjust this value based on your calculation
        }

        private float CalculateNutrientUptakeRate(Plant plant)
        {
            // Calculate nutrient uptake rate based on nutrient needs and availability
            float nitrogenUptakeRate = plant.nitrate_ppm[1] * plant.nutrientUptakeRate;
            float phosphorusUptakeRate = plant.phosphorus_ppm[1] * plant.nutrientUptakeRate;
            float potassiumUptakeRate = plant.potassium_ppm[1] * plant.nutrientUptakeRate;

            // Sum up the nutrient uptake rates
            float totalNutrientUptakeRate = nitrogenUptakeRate + phosphorusUptakeRate + potassiumUptakeRate;

            return totalNutrientUptakeRate;
        }


        private float CalculateLightIntensity(Vector3 plantPosition)
        {
            // Calculate light intensity based on position and time of day
            // For now, let's return a placeholder value
            return 1000.0f; // Adjust this value based on your calculation
        }

        private float GetCurrentTemperature()
        {
            return 25.0f;
        }
    }
}
