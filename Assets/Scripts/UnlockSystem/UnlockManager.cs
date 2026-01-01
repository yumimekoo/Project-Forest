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
        runtimeDatabase.allFurniture = new List<FurnitureSO>(Resources.LoadAll<FurnitureSO>("ScriptableObjectsData/Furniture"));
        RecalculateUnlocks();
    }

    // --- UNLOCK METHODS ---

    public void UnlockItem(ItemDataSO item)
    {
        if (!runtimeDatabase.unlockedItems.Contains(item))
        {
            runtimeDatabase.unlockedItems.Add(item);
            Debug.Log("[Unlock] item unlocked: " + item.name);
        }
        RecalculateUnlocks();
    }

    public void UnlockRecipe(DrinkRuleSO recipe)
    {
        if (!runtimeDatabase.unlockedRecipes.Contains(recipe))
        {
            runtimeDatabase.unlockedRecipes.Add(recipe);
            Debug.Log("[Unlock] Recipe unlocked: " + recipe.name);
        }
        RecalculateUnlocks();
    }

    public void ActivateRecipe(DrinkRuleSO recipe)
    {
        if (!runtimeDatabase.activeRecipes.Contains(recipe))
        {
            runtimeDatabase.activeRecipes.Add(recipe);
            Debug.Log("[Activate] Recipe activated: " + recipe.name);
        }
        RecalculateUnlocks();
    }

    public void UnlockFurniture(FurnitureSO furniture)
    {
        if (!runtimeDatabase.unlockedFurniture.Contains(furniture))
        {
            runtimeDatabase.unlockedFurniture.Add(furniture);
            Debug.Log("[Unlock] Furniture unlocked: " + furniture.name);
        }
    }

    bool CanUnlockRule(DrinkRuleSO rule, List<ItemDataSO> unlocked)
    {
        return unlocked.Contains(rule.requiredState) &&
               unlocked.Contains(rule.addedIngredient);
    }

    void RecalculateUnlocks()
    {
        bool unlockedSomething;
        do
        {
            unlockedSomething = false;
            foreach (var rule in runtimeDatabase.activeRecipes)
            {
                if(runtimeDatabase.unlockedRecipes.Contains(rule))
                    continue;

                if (CanUnlockRule(rule, runtimeDatabase.unlockedItems))
                {
                    runtimeDatabase.unlockedRecipes.Add(rule);
                    Debug.Log("[Unlock] Recipe unlocked: " + rule.name);

                    if (!runtimeDatabase.unlockedItems.Contains(rule.resultingState))
                    {
                        runtimeDatabase.unlockedItems.Add(rule.resultingState);
                        unlockedSomething = true;
                        Debug.Log("[Unlock] item unlocked: " + rule.resultingState.name);
                    }
                }
            }
        } while (unlockedSomething);
    }

    // --- CHECK UNLOCKED METHODS ---

    public bool IsItemUnlocked(ItemDataSO item)
        => runtimeDatabase.unlockedItems.Contains(item);
    public bool IsRecipeUnlocked(DrinkRuleSO recipe)
        => runtimeDatabase.unlockedRecipes.Contains(recipe);
    public bool IsFurnitureUnlocked(FurnitureSO furniture)
        => runtimeDatabase.unlockedFurniture.Contains(furniture);

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
        foreach (var furniture in runtimeDatabase.unlockedFurniture)
            saveData.unlockedFurnitureIDs.Add(furniture.numericID);
        return saveData;
    }

    public void ApplySaveData(UnlockSaveData saveData)
    {
        if(saveData.unlockedRecipeIDs.Count == 0 
            && saveData.unlockedItemIDs.Count == 0 
            && saveData.activeRecipeIDs.Count == 0 
            && saveData.unlockedFurnitureIDs.Count == 0)
        {
            Debug.Log("No unlock data to apply.");
            return;
        }
        runtimeDatabase.unlockedItems.Clear();
        runtimeDatabase.unlockedRecipes.Clear();
        runtimeDatabase.activeRecipes.Clear();
        runtimeDatabase.unlockedFurniture.Clear();

        foreach (var itemID in saveData.unlockedItemIDs)
        {
            //Debug.Log($"Processing item ID: {itemID}");
            var item = runtimeDatabase.allItems.Find(i => i.id == itemID);
            if (item != null)
            {
                //Debug.Log($"Unlocking item: {item.name}");
                runtimeDatabase.unlockedItems.Add(item);
            }      
        }
        foreach (var recipeID in saveData.activeRecipeIDs)
        {
            //Debug.Log($"Processing active recipe ID: {recipeID}");
            var recipe = runtimeDatabase.allRecipes.Find(r => r.id == recipeID);
            if (recipe != null)
            {
                //Debug.Log($"Activating recipe: {recipe.name}");
                runtimeDatabase.activeRecipes.Add(recipe);
            }
        }
        foreach (var recipeID in saveData.unlockedRecipeIDs)
        {
            //Debug.Log($"Processing recipe ID: {recipeID}");
            var recipe = runtimeDatabase.allRecipes.Find(r => r.id == recipeID);
            if (recipe != null)
            {
                //Debug.Log($"Unlocking recipe: {recipe.name}");
                runtimeDatabase.unlockedRecipes.Add(recipe);
            }
                
        }
        foreach (var furnitureID in saveData.unlockedFurnitureIDs)
        {
            var furniture = runtimeDatabase.allFurniture.Find(f => f.numericID == furnitureID);
            if (furniture != null)
            {
                runtimeDatabase.unlockedFurniture.Add(furniture);
            }
        }
    }
}
