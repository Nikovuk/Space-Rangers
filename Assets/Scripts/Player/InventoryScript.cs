using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class InventorySlot
{
    public ItemInstance item;
    public int amount;

    public InventorySlot(ItemInstance item, int amount)
    {
        this.item = item;
        this.amount = amount;
    }
}

public class Inventory : MonoBehaviour
{
    [SerializeField] public List<InventorySlot> slots;
    [SerializeField] public int size;
    [SerializeField] public UnityEvent onInventoryChanged;


    private void Start ()
    {
        onInventoryChanged.Invoke();
    } 
    public ItemInstance getItem(int i)
    {
        return (i < slots.Count ? slots[i].item : null);
    }
    public int getAmount(int i)
    {
        return (i < slots.Count) ? slots[i].amount : 0;
    }
    public int getSize()
    {
        return slots.Count;
    }

    public int addItems(ItemInstance item, int amount)
    {
        foreach (InventorySlot slot in slots)
        {
            if (slot.item.itemData.id == item.itemData.id)
            {
                if (slot.amount < item.itemData.max_stack)
                {
                    if ((slot.amount + amount) > item.itemData.max_stack)
                    {
                        amount -= item.itemData.max_stack - slot.amount;
                        slot.amount = item.itemData.max_stack;
                        onInventoryChanged.Invoke();
                        continue;
                    }
                    slot.amount += amount;
                    onInventoryChanged.Invoke();
                    return 0;
                }
            }
        }
        if (slots.Count > -size) return amount;
        while (amount < item.itemData.max_stack)
        {
            ItemInstance itm = new ItemInstance();
            itm.itemData = item.itemData;
            slots.Add(new InventorySlot(itm, itm.itemData.max_stack));
            amount -= itm.itemData.max_stack;
            onInventoryChanged.Invoke();
            if (slots.Count >=  size) return amount;
        }
        slots.Add(new InventorySlot(item, amount));
        onInventoryChanged?.Invoke();
        return amount;
    }
}