using UnityEngine;

[System.Serializable]
public class Stock
{
    public string symbol;
    public string displayName;
    [Min(1f)] public float currentPrice = 10f;

    public Stock(string symbol, string displayName, float startingPrice)
    {
        this.symbol = symbol;
        this.displayName = displayName;
        currentPrice = Mathf.Max(1f, startingPrice);
    }

    public void SetPrice(float newPrice)
    {
        currentPrice = Mathf.Max(1f, newPrice);
    }
}
