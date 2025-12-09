using System.Collections.Generic;
using UnityEngine;

public class FurnitureDatabase : MonoBehaviour
{
    public static FurnitureDatabase Instance { get; private set; }
    private Dictionary<string, FurnitureSO> furnitureItems;
    public FurnitureSO Get(string id) => 
        furnitureItems.TryGetValue(id, out var item) ? item : null;

    private void Awake()
    {
        Instance = this;
        LoadDatabase();
    }

    private void LoadDatabase()
    {
        furnitureItems = new Dictionary<string, FurnitureSO>();
        FurnitureSO[] furnitureArray = Resources.LoadAll<FurnitureSO>("ScriptableObjectsData/Furniture");

        foreach (var furniture in furnitureArray)
        {
            if(!furnitureItems.ContainsKey(furniture.id))
            {
                furnitureItems.Add(furniture.id, furniture);
            }
        }

        Debug.Log($"Loaded {furnitureItems.Count} furniture items into the database.");
    }

}
