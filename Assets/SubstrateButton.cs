using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class SubstrateButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string substrateName; // Name of the substrate
    public SubstrateDataPanel substrateDataPanel; // Panel to display substrate data
    public TMP_Text playerMoneyText; // Text field to display player's current money

    private bool isPointerOver; // Flag to check if the pointer is over the button
    private JSONLoader jsonLoader; // Reference to the JSONLoader component
    private CurrencyManager currencyManager; // Reference to the CurrencyManager component
    private Substrate currentSubstrate; // Currently selected substrate

    private void Start()
    {
        // Initialize components
        jsonLoader = FindObjectOfType<JSONLoader>();
        currencyManager = FindObjectOfType<CurrencyManager>();
        substrateDataPanel.SetActive(false);
    }

    private void Update()
    {
        if (isPointerOver && !substrateDataPanel.IsActive())
        {
            UpdateSubstrateDataPanel();
            substrateDataPanel.SetActive(true);
        }
        else if (!isPointerOver && substrateDataPanel.IsActive())
        {
            substrateDataPanel.SetActive(false);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isPointerOver = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isPointerOver = false;
    }

    private void UpdateSubstrateDataPanel()
    {
        if (jsonLoader != null)
        {
            Substrate substrateData = jsonLoader.GetSubstrateDataByName(substrateName);
            if (substrateData != null)
            {
                currentSubstrate = substrateData;
                substrateDataPanel.UpdateSubstrateData(substrateData);
            }
            else
            {
                Debug.LogError("Substrate data is null for substrateName: " + substrateName);
            }
        }
        else
        {
            Debug.LogError("JSONLoader is null in SubstrateButton");
        }
    }

    public void OnSubstrateButtonClick()
    {
        if (currentSubstrate != null && currencyManager.CanAfford((decimal)currentSubstrate.price_usd))
        {
            currencyManager.SubtractUSD((decimal)currentSubstrate.price_usd);
            UpdatePlayerMoneyText();
            SoundManager.Instance.PlayKachingSound();
            Debug.Log("Purchase successful!");
        }
        else
        {
            SoundManager.Instance.PlayInsufficientFundsSound();
            Debug.Log("Insufficient funds!");
        }
    }

    private void UpdatePlayerMoneyText()
    {
        if (playerMoneyText != null && currencyManager != null)
        {
            playerMoneyText.text = "$ " + currencyManager.CurrentUSD.ToString("F2");
        }
    }
}
