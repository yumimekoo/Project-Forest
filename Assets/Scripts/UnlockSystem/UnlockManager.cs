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

        Initialize();

    }

    private void Initialize()
    {
        // all recipes from Resource folder 
        runtimeDatabase.allRecipes = new List<DrinkRuleSO>(Resources.LoadAll<DrinkRuleSO>("ScriptableObjectsData/RecipeRules"));
        runtimeDatabase.activeRecipes = new List<DrinkRuleSO>(Resources.LoadAll<DrinkRuleSO>("ScriptableObjectsData/RecipeRules/ActiveDefault"));
        runtimeDatabase.allItems = new List<ItemDataSO>(Resources.LoadAll<ItemDataSO>("ScriptableObjectsData/ItemData"));
    }

    // --- UNLOCK METHODS ---

    public void UnlockItem(ItemDataSO item)
    {
        if (!runtimeDatabase.unlockedItems.Contains(item))
        {
            runtimeDatabase.unlockedItems.Add(item);
            Debug.Log("[Unlock] item unlocked: " + item.name);
        }

    }
    //public void UnlockRecipe(DrinkRuleSO recipe)
    //{
    //    if (!runtimeDatabase.unlockedRecipes.Contains(recipe))
    //    {
    //        runtimeDatabase.unlockedRecipes.Add(recipe);
    //        Debug.Log("[Unlock] Recipe unlocked: " + recipe.name);
    //    }
    //}

    // --- CHECK UNLOCKED METHODS ---

    public bool IsItemUnlocked(ItemDataSO item)
        => runtimeDatabase.unlockedItems.Contains(item);
    public bool IsRecipeUnlocked(DrinkRuleSO recipe)
        => runtimeDatabase.unlockedRecipes.Contains(recipe);

    // --- SAVING / LOADING METHODS ---

    public UnlockSaveData GetSaveData()
    {
        UnlockSaveData saveData = new UnlockSaveData();
        foreach (var item in runtimeDatabase.unlockedItems)
            saveData.unlockedItemIDs.Add(item.id);
        foreach (var recipe in runtimeDatabase.unlockedRecipes)
            saveData.unlockedRecipeIDs.Add(recipe.id);
        foreach (var recipe in runtimeDatabase.activeRecipes)
            saveData.activeRecipeIDs.Add(recipe.id);
        return saveData;
    }

    public void ApplySaveData(UnlockSaveData saveData)
    {
        if(saveData.unlockedRecipeIDs.Count == 0 && saveData.unlockedItemIDs.Count == 0)
        {
            Debug.Log("No unlock data to apply.");
            return;
        }
        runtimeDatabase.unlockedItems.Clear();
        runtimeDatabase.unlockedRecipes.Clear();
        runtimeDatabase.activeRecipes.Clear();

        foreach (var itemID in saveData.unlockedItemIDs)
        {
            Debug.Log($"Processing item ID: {itemID}");
            var item = runtimeDatabase.allItems.Find(i => i.id == itemID);
            if (item != null)
            {
                Debug.Log($"Unlocking item: {item.name}");
                runtimeDatabase.unlockedItems.Add(item);
            }      
        }
        foreach (var recipeID in saveData.activeRecipeIDs)
        {
            Debug.Log($"Processing active recipe ID: {recipeID}");
            var recipe = runtimeDatabase.allRecipes.Find(r => r.id == recipeID);
            if (recipe != null)
            {
                Debug.Log($"Activating recipe: {recipe.name}");
                runtimeDatabase.activeRecipes.Add(recipe);
            }
        }
        foreach (var recipeID in saveData.unlockedRecipeIDs)
        {
            Debug.Log($"Processing recipe ID: {recipeID}");
            var recipe = runtimeDatabase.allRecipes.Find(r => r.id == recipeID);
            if (recipe != null)
            {
                Debug.Log($"Unlocking recipe: {recipe.name}");
                runtimeDatabase.unlockedRecipes.Add(recipe);
            }
                
        }
    }
}
