using UnityEngine;
using TMPro;

public class HUDScript : MonoBehaviour
{
    public TMP_Text ammoText;
    public TMP_Text fuelText;
    public TMP_Text creditsText;
    public TMP_Text cargoText;
    public PlayerResources playerResources;


    void Start()
    {
    }

    void Update()
    {
        cargoText.text = "Cargo: " + playerResources.cargo.ToString();
        ammoText.text = "Ammo: " + playerResources.ammo.ToString();
        fuelText.text = "Fuel: " + playerResources.fuel.ToString("F1");
        creditsText.text = "Credits : " + playerResources.credits.ToString();
    }
}
