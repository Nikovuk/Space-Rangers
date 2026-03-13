using UnityEngine;

public class PlanetScript : MonoBehaviour
{
    [SerializeField] private StockMarketUI stockMarketUI;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || stockMarketUI == null)
        {
            return;
        }

        stockMarketUI.OpenPlanetShop();
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
