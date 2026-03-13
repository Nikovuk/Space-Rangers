using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class StockMarketUI : MonoBehaviour
{
    public StockMarketManager marketManager;
    public PlayerPortfolio playerPortfolio;
    public TMP_Text marketText;
    public TMP_Text portfolioText;

    [Header("Panels")]
    public GameObject stocksCornerPanel;
    public GameObject stockShopPanel;

    [Header("Shop List")]
    public Transform rowsRoot;
    public StockMarketRowUI rowPrefab;
    [Min(1)] public int tradeAmount = 1;
    private readonly List<StockMarketRowUI> rows = new List<StockMarketRowUI>();

    private void OnEnable()
    {
        if (marketManager != null)
        {
            marketManager.OnPricesUpdated += Refresh;
        }

        BuildRows();
        Refresh();
    }

    private void OnDisable()
    {
        if (marketManager != null)
        {
            marketManager.OnPricesUpdated -= Refresh;
        }
    }

    public void OpenStockShop()
    {
        if (stocksCornerPanel != null)
        {
            stocksCornerPanel.SetActive(false);
        }

        if (stockShopPanel != null)
        {
            stockShopPanel.SetActive(true);
        }

        Refresh();
    }

    public void CloseStockShop()
    {
        if (stockShopPanel != null)
        {
            stockShopPanel.SetActive(false);
        }

        if (stocksCornerPanel != null)
        {
            stocksCornerPanel.SetActive(true);
        }
    }

    public void Refresh()
    {
        if (marketText != null)
        {
            marketText.text = BuildMarketText();
        }

        if (portfolioText != null)
        {
            portfolioText.text = BuildPortfolioText();
        }

        RefreshRows();
    }

    private void BuildRows()
    {
        if (marketManager == null || rowPrefab == null || rowsRoot == null || rows.Count > 0)
        {
            return;
        }

        for (int i = 0; i < marketManager.stocks.Count; i++)
        {
            Stock stock = marketManager.stocks[i];
            StockMarketRowUI row = Instantiate(rowPrefab, rowsRoot);
            string symbol = stock.symbol;
            row.Setup(() => Buy(symbol), () => Sell(symbol));
            rows.Add(row);
        }
    }

    private void RefreshRows()
    {
        if (marketManager == null)
        {
            return;
        }

        if (rows.Count == 0)
        {
            BuildRows();
        }

        int count = Mathf.Min(rows.Count, marketManager.stocks.Count);
        for (int i = 0; i < count; i++)
        {
            Stock stock = marketManager.stocks[i];
            int shares = playerPortfolio == null ? 0 : playerPortfolio.GetShares(stock.symbol);
            rows[i].SetData(stock.symbol + " " + stock.currentPrice.ToString("F2") + " | You: " + shares);
        }
    }

    private void Buy(string symbol)
    {
        if (marketManager != null && playerPortfolio != null)
        {
            marketManager.TryBuy(playerPortfolio, symbol, tradeAmount);
            Refresh();
        }
    }

    private void Sell(string symbol)
    {
        if (marketManager != null && playerPortfolio != null)
        {
            marketManager.TrySell(playerPortfolio, symbol, tradeAmount);
            Refresh();
        }
    }

    private string BuildMarketText()
    {
        if (marketManager == null)
        {
            return "No market connected.";
        }

        StringBuilder builder = new StringBuilder();
        for (int i = 0; i < marketManager.stocks.Count; i++)
        {
            Stock stock = marketManager.stocks[i];
            builder.Append(stock.symbol)
                .Append(": ")
                .Append(stock.currentPrice.ToString("F2"))
                .AppendLine();
        }

        return builder.ToString();
    }

    private string BuildPortfolioText()
    {
        if (playerPortfolio == null)
        {
            return "No portfolio connected.";
        }

        StringBuilder builder = new StringBuilder();
        for (int i = 0; i < playerPortfolio.holdings.Count; i++)
        {
            PlayerPortfolio.Holding holding = playerPortfolio.holdings[i];
            if (holding.shares <= 0)
            {
                continue;
            }

            builder.Append(holding.symbol)
                .Append(": ")
                .Append(holding.shares)
                .AppendLine(" shares");
        }

        return builder.Length == 0 ? "No holdings." : builder.ToString();
    }
}
