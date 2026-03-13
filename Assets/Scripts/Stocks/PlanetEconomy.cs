using UnityEngine;

public class PlanetEconomy : MonoBehaviour
{
    [Range(-0.5f, 1.5f)] public float demandModifier;

    public float GetPriceMultiplier()
    {
        return Mathf.Max(0.5f, 1f + demandModifier);
    }
}
