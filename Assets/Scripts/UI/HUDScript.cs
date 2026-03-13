using UnityEngine;
using TMPro;
using System.Text;

public class HUDScript : MonoBehaviour
{
    public TMP_Text ammoText;
    public TMP_Text fuelText;
    public TMP_Text creditsText;
    public TMP_Text cargoText;
    public TMP_Text planetGoodsText;
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

        if (planetGoodsText != null)
        {
            planetGoodsText.text = BuildPlanetGoodsText();
        }
    }

    private string BuildPlanetGoodsText()
    {
        if (playerResources == null || playerResources.planetGoods == null || playerResources.planetGoods.Count == 0)
        {
            return "Planet goods: -";
        }

        StringBuilder builder = new StringBuilder("Planet goods: ");
        bool hasAny = false;

        for (int i = 0; i < playerResources.planetGoods.Count; i++)
        {
            PlayerResources.PlanetGoods goods = playerResources.planetGoods[i];
            if (goods == null || goods.amount <= 0)
            {
                continue;
            }

            if (hasAny)
            {
                builder.Append(" | ");
            }

            builder.Append(goods.symbol)
                .Append(" ")
                .Append(goods.amount)
                .Append("/")
                .Append(PlayerResources.MaxPlanetGoodsPerPlanet);
            hasAny = true;
        }

        return hasAny ? builder.ToString() : "Planet goods: -";
    }
}
