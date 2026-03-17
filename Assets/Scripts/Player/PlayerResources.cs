using UnityEngine;

public class PlayerResources : MonoBehaviour
{
    public const int MaxPlanetGoodsPerType = 3;

    [Header("Hull")]
    public float maxHull = 100f;

    [Header("Resources")]
    public int credits = 0;
    public int ammo = 50;
    public float fuel = 100f;
    public int cargo = 0;
    public int oreFromPlanetA = 0;
    public int alloysFromPlanetB = 0;
    public int toolsFromPlanetV = 0;

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

    public int GetPlanetGoodsAmount(PlanetTradeType type)
    {
        switch (type)
        {
            case PlanetTradeType.PlanetA:
                return oreFromPlanetA;
            case PlanetTradeType.PlanetB:
                return alloysFromPlanetB;
            default:
                return toolsFromPlanetV;
        }
    }

    public bool TryBuyPlanetGoods(PlanetTradeType type, int price)
    {
        if (GetPlanetGoodsAmount(type) >= MaxPlanetGoodsPerType || !SpendCredits(price))
        {
            return false;
        }

        AddPlanetGoods(type, 1);
        return true;
    }

    public bool TrySellPlanetGoods(PlanetTradeType type, int price)
    {
        if (GetPlanetGoodsAmount(type) <= 0)
        {
            return false;
        }

        AddPlanetGoods(type, -1);
        AddCredits(price);
        return true;
    }

    private void AddPlanetGoods(PlanetTradeType type, int delta)
    {
        switch (type)
        {
            case PlanetTradeType.PlanetA:
                oreFromPlanetA = Mathf.Clamp(oreFromPlanetA + delta, 0, MaxPlanetGoodsPerType);
                break;
            case PlanetTradeType.PlanetB:
                alloysFromPlanetB = Mathf.Clamp(alloysFromPlanetB + delta, 0, MaxPlanetGoodsPerType);
                break;
            default:
                toolsFromPlanetV = Mathf.Clamp(toolsFromPlanetV + delta, 0, MaxPlanetGoodsPerType);
                break;
        }
    }

    public static string GetPlanetGoodsLabel(PlanetTradeType type)
    {
        switch (type)
        {
            case PlanetTradeType.PlanetA:
                return "Руда";
            case PlanetTradeType.PlanetB:
                return "Сплавы";
            default:
                return "Инструменты";
        }
    }

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
}