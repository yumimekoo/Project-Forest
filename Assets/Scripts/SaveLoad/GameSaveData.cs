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
    public List<ItemSaveData> itemInventory = new();
    public List<FriendshipSaveData> friendships = new();
    public DaySaveData currentDay;
}

// subclasses for GameSaveData
[System.Serializable]
public class UnlockSaveData
{
    public List<int> unlockedItemIDs = new();
    public List<int> activeRecipeIDs = new();
    public List<int> unlockedRecipeIDs = new();
    public List<int> unlockedFurnitureIDs = new();
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

[System.Serializable]
public class ItemSaveData
{
    public int id;
    public int amount;
    public ItemSaveData(int id, int amount)
    {
        this.id = id;
        this.amount = amount;
    }
}

[System.Serializable]
public class FriendshipSaveData
{
    public string npcID;
    public int xp;
    public int level;
    public FriendshipSaveData(string npcID, int xp, int level)
    {
        this.npcID = npcID;
        this.xp = xp;
        this.level = level;
    }
}

[System.Serializable]
public class DaySaveData
{
    public int day;
    public int weekDay;
    public int week;

    public DaySaveData(int day, int weekDay, int week)
    {
        this.day = day;
        this.weekDay = weekDay;
        this.week = week;
    }
}
