using System.Collections.Generic;
using UnityEngine;

public class FurnitureDatabase : MonoBehaviour
{
    public static FurnitureDatabase Instance;

    [Header("Loaded automatically, no manual setup needed")]
    public List<FurnitureSO> items = new();

    private Dictionary<int, FurnitureSO> lookup = new();

    private void Awake()
    {
        Instance = this;
        LoadAllFurniture();
        BuildLookup();
    }

    private void LoadAllFurniture()
    {
        items.Clear();

        FurnitureSO[] loaded = Resources.LoadAll<FurnitureSO>("ScriptableObjectsData/Furniture");

        items.AddRange(loaded);
        //Debug.Log($"[FurnitureDatabase] Loaded {items.Count} furniture items from Resources.");
    }

    private void BuildLookup()
    {
        lookup.Clear();

        foreach (var item in items)
        {
            if (lookup.ContainsKey(item.numericID))
            {
                Debug.LogWarning($"Duplicate Furniture ID detected: {item.name} (ID {item.id})");
                continue;
            }

            lookup[item.numericID] = item;
        }
    }

    public FurnitureSO GetByID(int id)
    {
        if (lookup.TryGetValue(id, out var so))
            return so;

        Debug.LogWarning($"No FurnitureSO found with ID: {id}");
        return null;
    }
}
