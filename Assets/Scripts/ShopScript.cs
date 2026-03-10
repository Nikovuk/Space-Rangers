using UnityEngine;

public class ShopScript : MonoBehaviour
{
    public PlayerResources playerResources;

    public int sellPricePerOre = 35;
    public int ammoPackCost = 30;
    public int ammoPackAmount = 5;
    public int fuelPackCost = 15;
    public float fuelPackAmount = 20f;

    bool playerInside = false;

    void Update()
    {
        if (!playerInside) return;

        if (Input.GetKeyDown(KeyCode.E))
            playerResources.SellAllCargo(sellPricePerOre);

        if (Input.GetKeyDown(KeyCode.Alpha1))
            if (playerResources.SpendCredits(ammoPackCost))
                playerResources.AddAmmo(ammoPackAmount);

        if (Input.GetKeyDown(KeyCode.Alpha2))
            if (playerResources.SpendCredits(fuelPackCost))
                playerResources.AddFuel(fuelPackAmount);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInside = true;
        Debug.Log("Player Inside");
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInside = false;
        Debug.Log("Player Outside");

    }
}
