using UnityEngine;
using System.Collections.Generic;

public class PlayerResources : MonoBehaviour
{
    [System.Serializable]
    public class PlanetGoods
    {
        public string symbol;
        public int amount;
    }

    public const int MaxPlanetGoodsPerPlanet = 3;
    
    [Header("Hull")]
    public float maxHull = 100f;

    [Header("Resources")]
    public int credits = 0;
    public int ammo = 50;
    public float fuel = 100f;
    public int cargo = 0;
    public List<PlanetGoods> planetGoods = new List<PlanetGoods>();

    private float hull;

    private void Awake()
    {
        hull = Mathf.Max(1f, maxHull);
    }

    public bool SpendAmmo(int amount)
    {
        if (ammo >= amount) { ammo -= amount; return true; }
        return false;
    }

    public bool UseFuel(float amount)
    {
        if (fuel >= amount) { fuel -= amount; return true; }
        return false;
    }

    public void AddCredits(int a) => credits += a;
    public bool SpendCredits(int a)
    {
        if (credits >= a) { credits -= a; return true; }
        return false;
    }

    public void AddAmmo(int a) => ammo += a;
    public void AddFuel(float a) => fuel += a;
    public void AddCargo(int a) => cargo += a;
    public int SellAllCargo(int pricePerUnit)
    {
        int gained = cargo * pricePerUnit;
        cargo = 0;
        credits += gained;
        return gained;
    }

    public void ReceiveDamage(float damage)
    {
        hull -= Mathf.Max(0f, damage);
        if (hull > 0f)
        {
            return;
        }

        Destroy(gameObject);
    }

    public int GetPlanetGoodsAmount(string symbol)
    {
        PlanetGoods goods = GetPlanetGoods(symbol);
        return goods == null ? 0 : goods.amount;
    }

    public bool TryBuyPlanetGoods(string symbol, float pricePerUnit, int amount)
    {
        if (amount <= 0)
        {
            return false;
        }

        PlanetGoods goods = GetOrCreatePlanetGoods(symbol);
        if (goods.amount + amount > MaxPlanetGoodsPerPlanet)
        {
            return false;
        }

        int totalCost = Mathf.CeilToInt(pricePerUnit * amount);
        if (!SpendCredits(totalCost))
        {
            return false;
        }

        goods.amount += amount;
        return true;
    }

    public bool TrySellPlanetGoods(string symbol, float pricePerUnit, int amount)
    {
        if (amount <= 0)
        {
            return false;
        }

        PlanetGoods goods = GetPlanetGoods(symbol);
        if (goods == null || goods.amount < amount)
        {
            return false;
        }

        goods.amount -= amount;
        AddCredits(Mathf.FloorToInt(pricePerUnit * amount));
        return true;
    }

    private PlanetGoods GetPlanetGoods(string symbol)
    {
        for (int i = 0; i < planetGoods.Count; i++)
        {
            if (planetGoods[i].symbol == symbol)
            {
                return planetGoods[i];
            }
        }

        return null;
    }

    private PlanetGoods GetOrCreatePlanetGoods(string symbol)
    {
        PlanetGoods goods = GetPlanetGoods(symbol);
        if (goods != null)
        {
            return goods;
        }

        goods = new PlanetGoods { symbol = symbol, amount = 0 };
        planetGoods.Add(goods);
        return goods;
    }
}
