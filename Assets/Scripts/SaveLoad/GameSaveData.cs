using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameSaveData
{
    public int currentMoney;
    public UnlockSaveData unlocks = new();
    public List<PlacedFurnitureData> placedFurniture = new();
}

[System.Serializable]
public class UnlockSaveData
{
    public List<int> unlockedFridgeItemIDs = new();
    public List<int> unlockedStorageItemIDs = new();
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
