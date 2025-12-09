
using System.Collections.Generic;
using UnityEngine;

public class FurnitureInventory : MonoBehaviour
{
    public static FurnitureInventory Instance;
    public List<InventoryItem> items = new();

    private void Awake()
    {
        Instance = this;
    }

    public void Add(string id, int amount = 1) // Adds the given amount of items with the given id
    {
        var entry = items.Find(i => i.id == id);
        if(entry != null)
        {
            entry.amount += amount;
        }
        else
        {
            items.Add(new InventoryItem { id = id, amount = amount });
        }
    }

    public bool Remove(string id) // Removes one item with the given id 
    {
        var entry = items.Find(i => i.id == id);
        if(entry == null || entry.amount <= 0)
        {
            return false;
        }
        entry.amount--;
        return true;
    }

    public bool Has(string id) // Checks if there is at least one item with the given id
    {
        return items.Exists(i => i.id == id && i.amount > 0);
    }
}
