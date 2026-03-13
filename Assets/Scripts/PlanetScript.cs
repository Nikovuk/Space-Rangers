using UnityEngine;

public class PlanetScript : MonoBehaviour
{
    [SerializeField] private string planetSymbol;
    [SerializeField] private StockMarketUI stockMarketUI;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || stockMarketUI == null)
        {
            return;
        }

        stockMarketUI.OpenPlanetShop(planetSymbol);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player") || stockMarketUI == null)
        {
            return;
        }

        stockMarketUI.ClosePlanetShop();
    }
}
