using System.Collections.Generic;
using UnityEngine;

public class ItemsInventory : MonoBehaviour
{
    public static ItemsInventory Instance;
    public DefaultInventorySO baseInventory;
    private Dictionary<int, int> itemInventory = new();
    

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void InitializeIfEmpty()
    {
        if (itemInventory.Count > 0 || !baseInventory)
            return;

        foreach (var stack in baseInventory.defaultItems)
        {
            if (!stack.item || stack.amount <= 0) continue;

            int id = stack.item.id;
            itemInventory[id] = stack.amount;
        }
    }

    public void Add(int itemID, int amount)
    {
        if (!itemInventory.ContainsKey(itemID))
        {
            itemInventory[itemID] = 0;
        }
        itemInventory[itemID] += amount;
    }

    public bool TryRemove(int itemID, int amount = 1)
    {
        if (itemInventory.ContainsKey(itemID) && itemInventory[itemID] >= amount)
        {
            itemInventory[itemID] -= amount;
            if (itemInventory[itemID] <= 0)
            {
                itemInventory.Remove(itemID);
            }
            return true;
        }
        return false;
    }

    public int GetAmount(int itemID)
    {
        return itemInventory.TryGetValue(itemID, out int amount) ? amount : 0;
    }

    public List<ItemSaveData> GetSaveData()
    {
        List<ItemSaveData> saveData = new List<ItemSaveData>();
        foreach (var kvp in itemInventory)
        {
            saveData.Add(new ItemSaveData(kvp.Key, kvp.Value));
            //Debug.Log($"[ItemsInventory] Save: ID {kvp.Key} = {kvp.Value}");
        }
        return saveData;
    }

    public void ApplySaveData(List<ItemSaveData> data)
    {
        if (data.Count == 0 || data == null)
        {
            //Debug.Log("[ItemsInventory] No item inventory data to apply.");
            InitializeIfEmpty();
            return;
        }
        itemInventory.Clear();
        foreach (var itemData in data)
        {
              itemInventory[itemData.id] = itemData.amount;
              //Debug.Log($"[ItemsInventory] Load: ID {itemData.id} = {itemData.amount}");
        }
        
        
    }
}
