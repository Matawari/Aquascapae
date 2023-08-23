using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class SpawnButtonHandler : MonoBehaviour
{
    public ObjectPlacementController placementController;
    public int prefabIndex;
    public TextMeshProUGUI moneyText;
    private Button button;
    private decimal itemPriceUSD;

    public JSONLoader jsonLoader;

    private void Start()
    {
        button = GetComponent<Button>();
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnButtonClick);

        jsonLoader = FindObjectOfType<JSONLoader>();
    }

    private void OnButtonClick()
    {
        if (!TimeController.IsGamePausedOrFastForwarded())
        {
            itemPriceUSD = GetItemPriceFromJSON();
            if (CurrencyManager.Instance.SubtractUSD(itemPriceUSD))
            {
                placementController.SelectedPrefabIndex = prefabIndex;

                // Move the SpawnObject() call here, after setting the prefab index
                placementController.SpawnObject();

                UpdateMoneyText();

                SoundManager.Instance.PlayKachingSound();
            }
            else
            {
                SoundManager.Instance.PlayInsufficientFundsSound();
            }
        }
    }



    private decimal GetItemPriceFromJSON()
    {
        string itemName = "Siamese Algae Eater";
        Fish fishData = jsonLoader.GetFishDataByName(itemName);
        if (fishData != null)
        {
            return fishData.price_usd;
        }

        return 0;
    }

    private void UpdateMoneyText()
    {
        if (moneyText != null)
        {
            moneyText.text = CurrencyManager.Instance.CurrentUSD.ToString("C");
        }
    }
}
