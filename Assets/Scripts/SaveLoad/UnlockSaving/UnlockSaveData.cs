using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UnlockSaveData
{
    public List<int> unlockedFridgeItemIDs = new();
    public List<int> unlockedStorageItemIDs = new();
    public List<int> unlockedRecipeIDs = new();
}
