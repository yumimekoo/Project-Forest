using System.Collections.Generic;
using UnityEngine;

// Class wich is saved to json 
[System.Serializable]
public class GameSaveData
{
    public int currentMoney;
    public UnlockSaveData unlocks = new();
    public List<PlacedFurnitureData> placedFurniture = new();
    public List<FurnitureInventorySaveData> furnitureInventory = new();
}

// subclasses for GameSaveData
[System.Serializable]
public class UnlockSaveData
{
    public List<int> unlockedItemIDs = new();
    public List<int> activeRecipeIDs = new();
    public List<int> unlockedRecipeIDs = new();
}

[System.Serializable]
public class SaveSlotInfo
{
    public bool exists;
    public int slotNumber;
    public string saveName;
    public string createdDate;
    public string lastModifiedDate;
}

[System.Serializable]
public class PlacedFurnitureData
{
    public int id;
    public int x;
    public int y;
    public int rotY;

    public PlacedFurnitureData(int id, int x, int y, int rotY)
    {
        this.id = id;
        this.x = x;
        this.y = y;
        this.rotY = rotY;
    }
}

[System.Serializable]
public class FurnitureInventorySaveData
{
    public int id;
    public int amount;

    public FurnitureInventorySaveData(int id, int amount)
    {
        this.id = id;
        this.amount = amount;
    }
}
