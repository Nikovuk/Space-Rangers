using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StockMarketRowUI : MonoBehaviour
{
    public TMP_Text stockText;
    public Button buyButton;
    public Button sellButton;

    public void Setup(Action onBuy, Action onSell)
    {
        if (buyButton != null)
        {
            buyButton.onClick.RemoveAllListeners();
            buyButton.onClick.AddListener(() => onBuy?.Invoke());
        }

        if (sellButton != null)
        {
            sellButton.onClick.RemoveAllListeners();
            sellButton.onClick.AddListener(() => onSell?.Invoke());
        }
    }

    public void SetData(string label)
    {
        if (stockText != null)
        {
            stockText.text = label;
        }
    }
}
