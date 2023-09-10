using UnityEngine;

public class SimulationManager : MonoBehaviour
{
    private float simulationTime;
    private float timeStep;
    private bool isSimulationRunning;
    private float currentTimeOfLightCycle;

    public WaterQualityParameters waterQualityParameters;
    public FishBehavior[] fishBehaviors;
    public PlantBehavior[] plantBehaviors;
    public AlgaeBehavior[] algaeBehaviors;
    public BacteriaBehavior[] bacteriaBehaviors;
    public LightCycle lightCycle;

    void Start()
    {
        simulationTime = 0.0f;
        timeStep = 0.1f;
        isSimulationRunning = false;
        currentTimeOfLightCycle = 0.0f;
    }

    void Update()
    {
        if (isSimulationRunning)
        {
            simulationTime += Time.deltaTime;
            currentTimeOfLightCycle += Time.deltaTime / 86400.0f;

            waterQualityParameters.UpdateParameters();
            waterQualityParameters.UpdateNutrientLevels();

            foreach (var fishBehavior in fishBehaviors)
            {
                float pHValue = waterQualityParameters.GetpH();
                float ammoniaValue = waterQualityParameters.GetAmmoniaLevel();
                float nitriteValue = waterQualityParameters.GetNitriteLevel();
                float nitrateValue = waterQualityParameters.GetNitrateLevel();
                float o2ProductionRate = waterQualityParameters.GetOxygenLevel();
                float currentTemperature = waterQualityParameters.GetTemperature();

                fishBehavior.ApplyWaterEffects(fishBehavior.fishData, pHValue, ammoniaValue, nitriteValue, nitrateValue, o2ProductionRate, currentTemperature);
            }

            foreach (var plantBehavior in plantBehaviors)
            {
                float consumedLight = CalculateConsumedLight(plantBehavior);
                float consumedNutrient = CalculateConsumedNutrient(plantBehavior);
                plantBehavior.UpdatePlantBehavior(consumedLight, consumedNutrient);
            }

            foreach (var algaeBehavior in algaeBehaviors)
            {
                algaeBehavior.UpdateAlgae();
            }

            foreach (var bacteriaBehavior in bacteriaBehaviors)
            {
                bacteriaBehavior.UpdateBacteria();
            }

            lightCycle.UpdateCycle(currentTimeOfLightCycle);
            HandleGameEvents();
        }
    }

    public void StartSimulation()
    {
        isSimulationRunning = true;
    }

    public void PauseSimulation()
    {
        isSimulationRunning = false;
    }

    private float CalculateConsumedLight(PlantBehavior plantBehavior)
    {
        return 0.0f;
    }

    private float CalculateConsumedNutrient(PlantBehavior plantBehavior)
    {
        return 0.0f;
    }

    private void HandleGameEvents()
    {
    }
}
