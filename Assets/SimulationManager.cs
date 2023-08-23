using UnityEngine;

public class SimulationManager : MonoBehaviour
{
    private float simulationTime;
    private float timeStep;
    private bool isSimulationRunning;
    private float currentTimeOfLightCycle; // 0.0f to 1.0f

    public WaterQualityParameters waterQualityParameters;
    public FishBehavior[] fishBehaviors;
    public PlantBehavior[] plantBehaviors;
    public AlgaeBehavior[] algaeBehaviors;
    public BacteriaBehavior[] bacteriaBehaviors;
    public LightCycle lightCycle;

    void Start()
    {
        simulationTime = 0.0f;
        timeStep = 0.1f; // Example time step of 0.1 seconds
        isSimulationRunning = false;
        currentTimeOfLightCycle = 0.0f;
    }

    void Update()
    {
        if (isSimulationRunning)
        {
            simulationTime += Time.deltaTime;
            currentTimeOfLightCycle += Time.deltaTime / 86400.0f; // Assuming a 24-hour light cycle

            // Update the water quality parameters
            waterQualityParameters.UpdateParameters();
            waterQualityParameters.UpdateNutrientLevels();

            foreach (var fishBehavior in fishBehaviors)
            {
                float pHValue = waterQualityParameters.GetpH();
                float ammoniaValue = waterQualityParameters.GetAmmoniaLevel();
                float nitriteValue = waterQualityParameters.GetNitriteLevel();
                float nitrateValue = waterQualityParameters.GetNitrateLevel();
                float o2ProductionRate = waterQualityParameters.GetOxygenLevel(); // Use GetOxygenLevel instead of GetOxygenProduction
                float co2AbsorptionRate = waterQualityParameters.GetCO2AbsorptionRate();
                float currentTemperature = waterQualityParameters.GetTemperature(); // Use GetTemperature instead of GetCurrentTemperature

                fishBehavior.ApplyWaterEffects(fishBehavior.fishData, pHValue, ammoniaValue, nitriteValue, nitrateValue, o2ProductionRate, co2AbsorptionRate, currentTemperature);
            }

            // Update each plant's behavior
            foreach (var plantBehavior in plantBehaviors)
            {
                float consumedLight = CalculateConsumedLight(plantBehavior);
                float consumedNutrient = CalculateConsumedNutrient(plantBehavior);
                plantBehavior.UpdatePlantBehavior(consumedLight, consumedNutrient);
            }

            // Update each algae's behavior
            foreach (var algaeBehavior in algaeBehaviors)
            {
                algaeBehavior.UpdateAlgae();
            }

            // Update each bacteria's behavior
            foreach (var bacteriaBehavior in bacteriaBehaviors)
            {
                bacteriaBehavior.UpdateBacteria();
            }

            // Update light cycle
            lightCycle.UpdateCycle(currentTimeOfLightCycle);

            // Handle game events, e.g., eutrophication or predator-prey interactions
            HandleGameEvents();
        }
    }

    // Method to start the simulation
    public void StartSimulation()
    {
        isSimulationRunning = true;
    }

    // Method to pause the simulation
    public void PauseSimulation()
    {
        isSimulationRunning = false;
    }

    private float CalculateConsumedLight(PlantBehavior plantBehavior)
    {
        // Implement your logic to calculate consumed light based on plantBehavior
        return 0.0f;
    }

    private float CalculateConsumedNutrient(PlantBehavior plantBehavior)
    {
        // Implement your logic to calculate consumed nutrient based on plantBehavior
        return 0.0f;
    }

    // Method to handle game events
    private void HandleGameEvents()
    {
        // Implement the logic for handling game events like eutrophication or predator-prey interactions
    }
}
