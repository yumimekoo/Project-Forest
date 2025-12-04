using System.Collections.Generic;
using UnityEngine;

public class UnlockManager : MonoBehaviour
{
    public static UnlockManager Instance;

    [SerializeField] private UnlockDatabaseSO unlockDatabaseTemplate;
    public UnlockDatabaseSO runtimeDatabase { get; private set; }

    private void Awake()
    {
        Instance = this;
        runtimeDatabase = ScriptableObject.Instantiate(unlockDatabaseTemplate);

    }

    // --- UNLOCK METHODS ---

    public void UnlockFridgeItem(ItemDataSO item)
    {
        if (!runtimeDatabase.unlockedFridgeItems.Contains(item))
        {
            runtimeDatabase.unlockedFridgeItems.Add(item);
            Debug.Log("[Unlock] Fridge item unlocked: " + item.name);
        }

    }
    public void UnlockStorageItem(ItemDataSO item)
    {
        if (!runtimeDatabase.unlockedStorageItems.Contains(item))
        {
            runtimeDatabase.unlockedStorageItems.Add(item);
            Debug.Log("[Unlock] Storage item unlocked: " + item.name);
        }
    }
    public void UnlockRecipe(DrinkRuleSO recipe)
    {
        if (!runtimeDatabase.unlockedRecipes.Contains(recipe))
        {
            runtimeDatabase.unlockedRecipes.Add(recipe);
            Debug.Log("[Unlock] Recipe unlocked: " + recipe.name);
        }
    }

    // --- CHECK UNLOCKED METHODS ---

    public bool IsFridgeItemUnlocked(ItemDataSO item)
        => runtimeDatabase.unlockedFridgeItems.Contains(item);
    public bool IsStorageItemUnlocked(ItemDataSO item)
        => runtimeDatabase.unlockedStorageItems.Contains(item);
    public bool IsRecipeUnlocked(DrinkRuleSO recipe)
        => runtimeDatabase.unlockedRecipes.Contains(recipe);

    // --- SAVING / LOADING METHODS ---

    public UnlockSaveData GetSaveData()
    {
        UnlockSaveData saveData = new UnlockSaveData();
        foreach (var item in runtimeDatabase.unlockedFridgeItems)
            saveData.unlockedFridgeItemIDs.Add(item.id);
        foreach (var item in runtimeDatabase.unlockedStorageItems)
            saveData.unlockedStorageItemIDs.Add(item.id);
        foreach (var recipe in runtimeDatabase.unlockedRecipes)
            saveData.unlockedRecipeIDs.Add(recipe.id);
        Debug.Log($"{saveData.unlockedFridgeItemIDs.Count} fridge items, {saveData.unlockedStorageItemIDs.Count} storage items, and {saveData.unlockedRecipeIDs.Count} recipes loaded into unlock database.");

        return saveData;
    }

    public void ApplySaveData(UnlockSaveData saveData)
    {
        if(saveData.unlockedRecipeIDs.Count == 0 && saveData.unlockedFridgeItemIDs.Count == 0 && saveData.unlockedStorageItemIDs.Count == 0)
        {
            Debug.Log("No unlock data to apply.");
            return;
        }
        runtimeDatabase.unlockedFridgeItems.Clear();
        runtimeDatabase.unlockedStorageItems.Clear();
        runtimeDatabase.unlockedRecipes.Clear();

        foreach (var itemID in saveData.unlockedFridgeItemIDs)
        {
            Debug.Log($"Processing fridge item ID: {itemID}");
            var item = runtimeDatabase.fridgeItems.Find(i => i.id == itemID);
            if (item != null)
            {
                Debug.Log($"Unlocking fridge item: {item.name}");
                runtimeDatabase.unlockedFridgeItems.Add(item);
            }      
        }
        foreach (var itemID in saveData.unlockedStorageItemIDs)
        {
            Debug.Log($"Processing storage item ID: {itemID}");
            var item = runtimeDatabase.storageItems.Find(i => i.id == itemID);
            if (item != null)
            {
                Debug.Log($"Unlocking storage item: {item.name}");
                runtimeDatabase.unlockedStorageItems.Add(item);
            }
                
        }
        foreach (var recipeID in saveData.unlockedRecipeIDs)
        {
            Debug.Log($"Processing recipe ID: {recipeID}");
            var recipe = runtimeDatabase.recipes.Find(r => r.id == recipeID);
            if (recipe != null)
            {
                Debug.Log($"Unlocking recipe: {recipe.name}");
                runtimeDatabase.unlockedRecipes.Add(recipe);
            }
                
        }
    }
}
