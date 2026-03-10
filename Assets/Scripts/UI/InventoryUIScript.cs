using NUnit.Framework;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;



public class InventoryUIScript : MonoBehaviour
{
    public Inventory inventory;
    [SerializeField] List<Image> icons = new List<Image>();
    [SerializeField] List<TMP_Text> amounts = new List<TMP_Text>();


    public void UpdateUI()
    {
        for (int i = 0; i < inventory.getSize(); i++)
        {
            icons[i].color = new Color(1, 1, 1, 1);
            icons[i].sprite = inventory.getItem(i).itemData.icon;
            amounts[i].text = (inventory.getAmount(i) > 1) ? inventory.getAmount(i).ToString() : "";
        }
        for (int i = inventory.getSize(); i< icons.Count; i++)
        {
            icons[i].color = new Color(1, 1, 1, 0);
            icons[i].sprite = null;
            amounts[i].text = "";
        } 
            
    }
}