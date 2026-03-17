using UnityEngine;
using UnityEngine.Events;

public class PlanetTradeTerminal : MonoBehaviour
{
    public PlanetTradeType planetType;
    public GameObject tradeCanvas;
    public PlanetTradeUI tradeUI;
    [SerializeField] public UnityEvent OnEnterTrade;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        PlayerResources resources = other.GetComponent<PlayerResources>();
        if (tradeUI != null)
        {
            tradeUI.playerResources = resources;
            tradeUI.SetPlanetType(planetType);
        }

        if (tradeCanvas != null)
        {
            tradeCanvas.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        if (tradeCanvas != null)
        {
            tradeCanvas.SetActive(false);
        }
    }
}