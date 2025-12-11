using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnlockDatabaseSO", menuName = "Scriptable Objects/UnlockDatabase")]
public class UnlockDatabaseSO : ScriptableObject
{
    [Header("Fridge Items")]
    public List<ItemDataSO> fridgeItems; // all frigde items
    public List<ItemDataSO> unlockedFridgeItems = new List<ItemDataSO>(); // unlocked fridge items

    [Header("Storage Items")]
    public List<ItemDataSO> storageItems; // all storage items
    public List<ItemDataSO> unlockedStorageItems = new List<ItemDataSO>(); // unlocked storage items
    /// public bool IsUnlocked(ItemDataSO item) => unlockedFridgeItems.Contains(item);
    [Header("Recipes")]
    public List<DrinkRuleSO> recipes; // all recipes
    public List<DrinkRuleSO> unlockedRecipes = new List<DrinkRuleSO>(); // unlocked recipes
    public List<ItemDataSO> GetUnlockedFridgeItems() => unlockedFridgeItems;
    public List<ItemDataSO> GetUnlockedStorageItems() => unlockedStorageItems;
    public List<DrinkRuleSO> GetUnlockedRecipes() => unlockedRecipes;
}
