using TMPro;
using UnityEngine;

public class PlanetTradeUI : MonoBehaviour
{
    public PlayerResources playerResources;
    public StockMarketManager marketManager;
    public TMP_Text titleText;
    public TMP_Text goodsText;
    public TMP_Text fuelText;

    [Header("Stock Symbols")]
    public string planetASymbol = "A";
    public string planetBSymbol = "B";
    public string planetVSymbol = "V";

    [Header("Fallback Prices")]
    public int goodsFallbackPrice = 20;
    public int fuelPackCost = 15;
    public float fuelPackAmount = 20f;

    private PlanetTradeType currentPlanetType;
    private float lastBuyPrice;

    public void SetPlanetType(PlanetTradeType type)
    {
        currentPlanetType = type;
        Refresh();
    }

    public void BuyGoods()
    {
        if (playerResources == null)
        {
            return;
        }

        float stockPrice = GetCurrentPlanetPrice();
        int buyPrice = Mathf.CeilToInt(stockPrice);
        if (!playerResources.TryBuyPlanetGoods(currentPlanetType, buyPrice))
        {
            return;
        }

        lastBuyPrice = stockPrice;
        string symbol = GetCurrentPlanetSymbol();
        if (marketManager != null && !string.IsNullOrEmpty(symbol))
        {
            marketManager.ApplyStockPriceDelta(symbol, stockPrice * 0.1f);
        }

        Refresh();
    }

    public void SellGoods()
    {
        if (playerResources == null)
        {
            return;
        }

        float stockPrice = GetCurrentPlanetPrice();
        int sellPrice = Mathf.FloorToInt(stockPrice);
        if (!playerResources.TrySellPlanetGoods(currentPlanetType, sellPrice))
        {
            return;
        }

        float priceForDelta = lastBuyPrice > 0f ? lastBuyPrice : stockPrice;
        string symbol = GetCurrentPlanetSymbol();
        if (marketManager != null && !string.IsNullOrEmpty(symbol))
        {
            marketManager.ApplyStockPriceDelta(symbol, -priceForDelta * 0.1f);
        }

        Refresh();
    }

    public void BuyFuel()
    {
        if (playerResources != null && playerResources.SpendCredits(fuelPackCost))
        {
            playerResources.AddFuel(fuelPackAmount);
            Refresh();
        }
    }

    public void Refresh()
    {
        if (playerResources == null)
        {
            return;
        }

        float stockPrice = GetCurrentPlanetPrice();

        if (titleText != null)
        {
            titleText.text = "Торговля: " + PlayerResources.GetPlanetGoodsLabel(currentPlanetType);
        }

        if (goodsText != null)
        {
            int amount = playerResources.GetPlanetGoodsAmount(currentPlanetType);
            goodsText.text = PlayerResources.GetPlanetGoodsLabel(currentPlanetType)
                + ": " + amount + "/" + PlayerResources.MaxPlanetGoodsPerType
                + " | Цена: " + stockPrice.ToString("F2");
        }

        if (fuelText != null)
        {
            fuelText.text = "Топливо: +" + fuelPackAmount.ToString("F0") + " за " + fuelPackCost;
        }
    }

    private float GetCurrentPlanetPrice()
    {
        if (marketManager != null)
        {
            string symbol = GetCurrentPlanetSymbol();
            if (!string.IsNullOrEmpty(symbol))
            {
                Stock stock = marketManager.GetStock(symbol);
                if (stock != null)
                {
                    return stock.currentPrice;
                }
            }
        }

        return goodsFallbackPrice;
    }

    private string GetCurrentPlanetSymbol()
    {
        switch (currentPlanetType)
        {
            case PlanetTradeType.PlanetA:
                return planetASymbol;
            case PlanetTradeType.PlanetB:
                return planetBSymbol;
            default:
                return planetVSymbol;
        }
    }
}