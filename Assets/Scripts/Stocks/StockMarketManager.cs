using System;
using System.Collections.Generic;
using UnityEngine;

public class StockMarketManager : MonoBehaviour
{
    public List<Stock> stocks = new List<Stock>();
    public PlanetEconomy planetEconomy;
    [Min(0f)] public float volatility = 0.05f;
    [Min(0.1f)] public float updateInterval = 3f;

    public event Action OnPricesUpdated;

    private float elapsed;

    private void Update()
    {
        elapsed += Time.deltaTime;
        if (elapsed < updateInterval)
        {
            return;
        }

        elapsed = 0f;
        UpdatePrices();
    }

    public Stock GetStock(string symbol)
    {
        for (int i = 0; i < stocks.Count; i++)
        {
            if (stocks[i].symbol == symbol)
            {
                return stocks[i];
            }
        }

        return null;
    }

    public bool TryBuy(PlayerPortfolio portfolio, string symbol, int amount)
    {
        Stock stock = GetStock(symbol);
        return stock != null && portfolio != null && portfolio.TryBuy(symbol, stock.currentPrice, amount);
    }

    public bool TrySell(PlayerPortfolio portfolio, string symbol, int amount)
    {
        Stock stock = GetStock(symbol);
        return stock != null && portfolio != null && portfolio.TrySell(symbol, stock.currentPrice, amount);
    }

    public bool ApplyTraderTransfer(string sourceSymbol, string destinationSymbol, bool hasCargo)
    {
        if (!hasCargo)
        {
            return false;
        }

        Stock sourceStock = GetStock(sourceSymbol);
        Stock destinationStock = GetStock(destinationSymbol);
        if (sourceStock == null || destinationStock == null)
        {
            return false;
        }

        float sourceDelta = sourceStock.currentPrice * 0.1f;
        sourceStock.SetPrice(sourceStock.currentPrice + sourceDelta);
        destinationStock.SetPrice(destinationStock.currentPrice - sourceDelta * 1.02f);

        OnPricesUpdated?.Invoke();
        return true;
    }

    public float GetTraderPurchasePrice(string symbol)
    {
        Stock stock = GetStock(symbol);
        return stock == null ? 0f : stock.currentPrice * 0.1f;
    }

    public bool ApplyStockPriceDelta(string symbol, float delta)
    {
        Stock stock = GetStock(symbol);
        if (stock == null)
        {
            return false;
        }

        stock.SetPrice(stock.currentPrice + delta);
        OnPricesUpdated?.Invoke();
        return true;
    }

    public void UpdatePrices()
    {
        float economyMultiplier = planetEconomy == null ? 1f : planetEconomy.GetPriceMultiplier();

        for (int i = 0; i < stocks.Count; i++)
        {
            float randomChange = UnityEngine.Random.Range(-volatility, volatility);
            float price = stocks[i].currentPrice * (1f + randomChange) * economyMultiplier;
            stocks[i].SetPrice(price);
        }

        OnPricesUpdated?.Invoke();
    }
}
