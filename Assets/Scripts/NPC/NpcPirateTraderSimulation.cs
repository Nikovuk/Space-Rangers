using System.Collections.Generic;
using UnityEngine;

public class NpcPirateTraderSimulation : MonoBehaviour
{
    [SerializeField] private PlanetEconomy planetEconomy;
    [SerializeField] private StockMarketManager stockMarketManager;

    [Header("Pirate Stocks")]
    [SerializeField] private List<string> pirateStockSymbols = new List<string> { "PIR" };
    [SerializeField] private float pirateStockDropOnPirateDestroyed = 0.03f;
    [SerializeField] private float traderStockDropOnTraderDestroyed = 0.03f;

    [Header("Loop")]
    [SerializeField] private float tickInterval = 1f;

    [Header("Interception")]
    [SerializeField] private float laserMaxDistance = 25f;
    [SerializeField] private float laserMinDistance = 10f;
    [SerializeField] private float laserShotInterval = 0.35f;
    [SerializeField] private float pirateDps = 6f;
    [SerializeField] private float traderDps = 2f;
    [SerializeField] private float traderEscapeSeconds = 3f;
    [SerializeField] private int maxTradersPerPirateTick = 1;
    [SerializeField] private GameObject laserPrefab;

    [Header("Economy Impact")]
    [SerializeField] private float demandPenaltyPerDestroyedTrader = 0.01f;

    private float tickTimer;

    private void Update()
    {
        tickTimer -= Time.deltaTime;
        if (tickTimer > 0f)
        {
            return;
        }

        tickTimer = Mathf.Max(0.05f, Mathf.Min(tickInterval, laserShotInterval));
        RunInterceptions();
    }

    private void RunInterceptions()
    {
        NpcPirateShip[] pirates = FindObjectsOfType<NpcPirateShip>();
        NpcTraderShip[] traders = FindObjectsOfType<NpcTraderShip>();

        if (pirates.Length == 0 || traders.Length == 0)
        {
            return;
        }

        float dpsStep = Mathf.Max(0.05f, laserShotInterval);
        float maxSqrDistance = laserMaxDistance * laserMaxDistance;
        float minSqrDistance = laserMinDistance * laserMinDistance;

        for (int i = 0; i < pirates.Length; i++)
        {
            NpcPirateShip pirate = pirates[i];
            if (pirate == null || pirate.IsDestroyed)
            {
                continue;
            }

            int attacked = 0;
            for (int t = 0; t < traders.Length && attacked < maxTradersPerPirateTick; t++)
            {
                NpcTraderShip trader = traders[t];
                if (trader == null || trader.IsDestroyed)
                {
                    continue;
                }

                if (pirate.CurrentRoute != null && trader.CurrentRoute != null && pirate.CurrentRoute != trader.CurrentRoute)
                {
                    continue;
                }

                float sqrDistance = (pirate.transform.position - trader.transform.position).sqrMagnitude;
                if (sqrDistance > maxSqrDistance)
                {
                    continue;
                }

                if (sqrDistance < minSqrDistance)
                {
                    trader.TriggerEscape(traderEscapeSeconds);
                    pirate.TriggerEscape(traderEscapeSeconds * 0.5f);
                    continue;
                }

                trader.TriggerEscape(traderEscapeSeconds);
                ShootLaser(pirate.transform, trader.transform.position, pirateDps * dpsStep);
                ShootLaser(trader.transform, pirate.transform.position, traderDps * dpsStep);
                attacked++;

                if (trader == null || trader.IsDestroyed)
                {
                    pirate.ReturnToBase();
                }
            }
        }
    }

    private void ShootLaser(Transform shooter, Vector3 targetPosition, float shipDamage)
    {
        if (laserPrefab == null || shooter == null)
        {
            return;
        }

        Vector3 spawnPosition = shooter.position + shooter.forward * 2f;
        Vector3 direction = (targetPosition - spawnPosition).normalized;
        if (direction.sqrMagnitude < 0.0001f)
        {
            direction = shooter.forward;
        }

        GameObject laser = Instantiate(laserPrefab, spawnPosition, Quaternion.LookRotation(direction, Vector3.up));
        LaserScript laserScript = laser.GetComponent<LaserScript>();
        if (laserScript != null)
        {
            laserScript.InitializeNpcShot(shipDamage, 0f);
        }
    }

    public void NotifyTraderDestroyed(NpcShipRoute traderRoute)
    {
        if (planetEconomy != null)
        {
            planetEconomy.demandModifier -= Mathf.Abs(demandPenaltyPerDestroyedTrader);
        }

        if (traderRoute == null)
        {
            return;
        }

        ApplyStockImpact(traderRoute.TraderCargoStockSymbol, -Mathf.Abs(traderStockDropOnTraderDestroyed));
    }

    public void NotifyPirateDestroyed()
    {
        ApplyPirateStockImpact(-Mathf.Abs(pirateStockDropOnPirateDestroyed));
    }

    private void ApplyStockImpact(string symbol, float change)
    {
        if (stockMarketManager == null || string.IsNullOrWhiteSpace(symbol))
        {
            return;
        }

        Stock stock = stockMarketManager.GetStock(symbol);
        if (stock == null)
        {
            return;
        }

        stock.SetPrice(stock.currentPrice * (1f + change));
        stockMarketManager.UpdatePrices();
    }

    private void ApplyPirateStockImpact(float change)
    {
        if (stockMarketManager == null || pirateStockSymbols == null || pirateStockSymbols.Count == 0)
        {
            return;
        }

        for (int i = 0; i < pirateStockSymbols.Count; i++)
        {
            ApplyStockImpact(pirateStockSymbols[i], change);
        }
    }
}
