using System.Collections.Generic;
using UnityEngine;

public class FurniturePlacementManager : MonoBehaviour
{
    public static FurniturePlacementManager Instance;
    public List<PlacedFurnitureData> placedFurniture = new();

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void RegisterPlacement(int id, Vector2Int cell, int rotY)
    {
        placedFurniture.Add(new PlacedFurnitureData(id, cell.x, cell.y, rotY));
    }

    public void RemovePlacement(Vector2Int cell)
    {
        placedFurniture.RemoveAll(i => i.x == cell.x && i.y == cell.y);
    }

    public void ClearAllPlacements()
    {
        placedFurniture.Clear();
    }

    public List<PlacedFurnitureData> GetSaveData()
    {
        return placedFurniture;
    }

    public void ApplySaveData(List<PlacedFurnitureData> data)
    {
        placedFurniture = data;
    }
}
