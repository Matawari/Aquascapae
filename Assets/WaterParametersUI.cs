using UnityEngine;
using TMPro;

public class WaterParametersUI : MonoBehaviour
{
    public WaterQualityParameters waterQualityParameters;

    public TMP_Text nitrateText;
    public TMP_Text potassiumText;
    public TMP_Text phosphorusText;
    public TMP_Text temperatureText;
    public TMP_Text pHText;
    public TMP_Text wasteLevelText;
    public TMP_Text nutrientLevelText;
    public TMP_Text algaePopulationText;
    public TMP_Text ammoniaLevelText;
    public TMP_Text oxygenLevelText;
    public TMP_Text nitriteText;
    public TMP_Text toxinLevelText;

    private void Update()
    {
        // Check if the waterQualityParameters reference is set.
        if (waterQualityParameters != null)
        {
            // Update the TextMeshPro Text components with the corresponding parameter values.
            nitrateText.text = $"Nitrate: {waterQualityParameters.GetCurrentNitrate()}";
            potassiumText.text = $"Potassium: {waterQualityParameters.GetCurrentPotassium()}";
            phosphorusText.text = $"Phosphorus: {waterQualityParameters.GetCurrentPhosphorus()}";
            temperatureText.text = $"Temperature: {waterQualityParameters.GetTemperature()}";
            pHText.text = $"pH: {waterQualityParameters.GetpH()}";
            wasteLevelText.text = $"Waste Level: {waterQualityParameters.GetWasteLevel()}";
            nutrientLevelText.text = $"Nutrient Level: {waterQualityParameters.GetNutrientLevel()}";
            algaePopulationText.text = $"Algae Population: {waterQualityParameters.GetAlgaePopulation()}";
            ammoniaLevelText.text = $"Ammonia Level: {waterQualityParameters.GetAmmoniaLevel()}";
            oxygenLevelText.text = $"Oxygen Level: {waterQualityParameters.GetOxygenLevel()}";
            nitriteText.text = $"Nitrite: {waterQualityParameters.GetNitriteLevel()}";
            toxinLevelText.text = $"Toxin Level: {waterQualityParameters.GetToxinLevel()}";
        }
        else
        {
            Debug.LogError("WaterQualityParameters reference is not set. Please assign it in the Inspector.");
        }
    }
}
