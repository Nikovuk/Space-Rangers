using UnityEngine;

public class PlayerResources : MonoBehaviour
{
    [Header("Resources")]
    public int credits = 0;
    public int ammo = 50;
    public float fuel = 100f;
    public int cargo = 0;

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
}
