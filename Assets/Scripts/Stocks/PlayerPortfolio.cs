using System.Collections.Generic;
using UnityEngine;

public class PlayerPortfolio : MonoBehaviour
{
    [System.Serializable]
    public class Holding
    {
        public string symbol;
        public int shares;
    }

    public PlayerResources playerResources;
    public List<Holding> holdings = new List<Holding>();

    public int GetShares(string symbol)
    {
        Holding holding = GetHolding(symbol);
        return holding == null ? 0 : holding.shares;
    }

    public bool TryBuy(string symbol, float price, int amount)
    {
        if (amount <= 0 || playerResources == null)
        {
            return false;
        }

        int totalCost = Mathf.CeilToInt(price * amount);
        if (!playerResources.SpendCredits(totalCost))
        {
            return false;
        }

        Holding holding = GetOrCreateHolding(symbol);
        holding.shares += amount;
        return true;
    }

    public bool TrySell(string symbol, float price, int amount)
    {
        if (amount <= 0 || playerResources == null)
        {
            return false;
        }

        Holding holding = GetHolding(symbol);
        if (holding == null || holding.shares < amount)
        {
            return false;
        }

        holding.shares -= amount;
        playerResources.AddCredits(Mathf.FloorToInt(price * amount));
        return true;
    }

    private Holding GetHolding(string symbol)
    {
        for (int i = 0; i < holdings.Count; i++)
        {
            if (holdings[i].symbol == symbol)
            {
                return holdings[i];
            }
        }

        return null;
    }

    private Holding GetOrCreateHolding(string symbol)
    {
        Holding holding = GetHolding(symbol);
        if (holding != null)
        {
            return holding;
        }

        holding = new Holding { symbol = symbol, shares = 0 };
        holdings.Add(holding);
        return holding;
    }
}
