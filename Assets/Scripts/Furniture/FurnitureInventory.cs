using System;
using System.Collections.Generic;
using UnityEngine;

public class FurnitureInventory : MonoBehaviour
{
    public static FurnitureInventory Instance;
    private Dictionary<int,int> inventory = new Dictionary<int,int>();

    [System.Serializable]
    public class DefaultItem
    {
        public int id;
        public int amount;
    }
    public List<DefaultItem> defaultInventory = new List<DefaultItem>();

    public event Action<int, int> OnInventoryChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        InitDefaultInventory();
    }

    public void InitDefaultInventory()
    {
        inventory.Clear();

        foreach (var item in defaultInventory)
        {
            inventory[item.id] = item.amount;
            Debug.Log($"[FurnitureInventory] Default: ID {item.id} = {item.amount}");
        }
    }

    public void Add(int id, int amount = 1)
    {
        if (!inventory.ContainsKey(id))
        {
            inventory[id] = 0;
        }
        inventory[id] += amount;
        OnInventoryChanged?.Invoke(id, inventory[id]);
    }

    public bool Remove(int id, int amount = 1)
    {
        if (!inventory.ContainsKey(id) || inventory[id] <= 0)
        {
            return false;
        }

        inventory[id] -= amount;
        OnInventoryChanged?.Invoke(id, inventory[id]);
        return true;
    }

    public int GetAmount(int id)
    {
        return inventory.ContainsKey(id) ? inventory[id] : 0;
    }

    public Dictionary<int, int> GetAll() => inventory;
    public void SetAll(Dictionary<int, int> data) => inventory = data;

    public List<FurnitureInventorySaveData> GetSaveData()
    {
        List<FurnitureInventorySaveData> saveData = new List<FurnitureInventorySaveData>();
        foreach (var kvp in inventory)
        {
            saveData.Add(new FurnitureInventorySaveData(kvp.Key, kvp.Value));
            Debug.Log($"[FurnitureInventory] Saved item ID {kvp.Key} with amount {kvp.Value}");
        }
        return saveData;
    }

    public void ApplySaveData(List<FurnitureInventorySaveData> data)
    {
        if (data == null || data.Count == 0)
        {
            Debug.Log("[FurnitureInventory] SaveData empty keeping default inventory.");
            return;
        }

        inventory.Clear();
        foreach (var item in data)
        {
            inventory[item.id] = item.amount;
            Debug.Log($"[FurnitureInventory] Loaded item ID {item.id} with amount {item.amount}");
        }
    }
}
