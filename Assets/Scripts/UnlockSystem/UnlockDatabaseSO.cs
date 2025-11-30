using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnlockDatabaseSO", menuName = "Unlock/UnlockDatabase")]
public class UnlockDatabaseSO : ScriptableObject
{
    public List<ItemDataSO> fridgeItems; // all frigde items
    public List<ItemDataSO> unlockedFridgeItems = new List<ItemDataSO>(); // unlocked fridge items

    public bool IsUnlocked(ItemDataSO item) => unlockedFridgeItems.Contains(item);

    public List<ItemDataSO> GetUnlockedFridgeItems() => unlockedFridgeItems;
}
