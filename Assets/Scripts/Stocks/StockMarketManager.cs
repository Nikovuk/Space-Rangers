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
