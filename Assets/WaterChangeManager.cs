using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WaterChangeManager : MonoBehaviour
{
    [SerializeField] private WaterQualityParameters waterQualityParameters;
    public GameObject waterChangePanel;
    public GameObject confirmationPanel1;
    public GameObject confirmationPanel2;
    public Slider waterChangeSlider;

    // UI TextMeshPro components for displaying the water parameters
    public TMP_Text nitrateText;
    public TMP_Text potassiumText;
    public TMP_Text phosphorusText;
    public TMP_Text pHText;
    public TMP_Text ammoniaText;
    public TMP_Text oxygenText;
    public TMP_Text nitriteText;

    private bool isWaterChangePanelOpen = false;
    private bool isConfirmationPanel1Open = true; // Set to true initially
    private bool isConfirmationPanel2Open = false;

    private void Start()
    {
        waterChangePanel.SetActive(false);
        confirmationPanel2.SetActive(false);

        if (waterChangeSlider != null)
        {
            waterChangeSlider.onValueChanged.AddListener(delegate { DisplayCurrentParameters(); });
            DisplayCurrentParameters();
        }
        else
        {
            Debug.LogError("WaterChangeSlider is not set in the WaterChangeManager.");
        }

        // Open Confirmation Panel 1 initially
        OpenConfirmationPanel1();
    }

    public void DisplayCurrentParameters()
    {
        nitrateText.text = $"Nitrate: {waterQualityParameters.GetCurrentNitrate()} >> {waterQualityParameters.GetForecastedNitrate(waterChangeSlider.value)}";
        potassiumText.text = $"Potassium: {waterQualityParameters.GetCurrentPotassium()} >> {waterQualityParameters.GetForecastedPotassium(waterChangeSlider.value)}";
        phosphorusText.text = $"Phosphorus: {waterQualityParameters.GetCurrentPhosphorus()} >> {waterQualityParameters.GetForecastedPhosphorus(waterChangeSlider.value)}";
        pHText.text = $"pH: {waterQualityParameters.GetCurrentpH()} >> {waterQualityParameters.GetForecastedpH(waterChangeSlider.value)}";
        ammoniaText.text = $"Ammonia: {waterQualityParameters.GetCurrentAmmonia()} >> {waterQualityParameters.GetForecastedAmmonia(waterChangeSlider.value)}";
        oxygenText.text = $"Oxygen: {waterQualityParameters.GetCurrentOxygen()} >> {waterQualityParameters.GetForecastedOxygen(waterChangeSlider.value)}";
        nitriteText.text = $"Nitrite: {waterQualityParameters.GetCurrentNitrite()} >> {waterQualityParameters.GetForecastedNitrite(waterChangeSlider.value)}";
    }

    public void OpenWaterChangePanel()
    {
        waterChangePanel.SetActive(true);
        isWaterChangePanelOpen = true;
    }

    public void CloseWaterChangePanel()
    {
        waterChangePanel.SetActive(false);
        isWaterChangePanelOpen = false;
    }

    public void OpenConfirmationPanel1()
    {
        confirmationPanel1.SetActive(true);
        isConfirmationPanel1Open = true;
    }

    public void CloseConfirmationPanel1()
    {
        confirmationPanel1.SetActive(false);
        isConfirmationPanel1Open = false;
    }

    public void OpenConfirmationPanel2()
    {
        confirmationPanel2.SetActive(true);
        isConfirmationPanel2Open = true;
    }

    public void CloseConfirmationPanel2()
    {
        confirmationPanel2.SetActive(false);
        isConfirmationPanel2Open = false;
    }

    public void ApplyWaterChange()
    {
        // Apply the changes to water parameters here.
        // You can add your logic for applying the changes.
    }
}
