using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnlockDatabaseSO", menuName = "Scriptable Objects/UnlockDatabase")]
public class UnlockDatabaseSO : ScriptableObject
{
    [Header("Items")]
    public List<ItemDataSO> allItems;
    public List<ItemDataSO> unlockedItems = new List<ItemDataSO>();

    [Header("Recipes")]
    public List<DrinkRuleSO> allRecipes; // all recipes
    public List<DrinkRuleSO> activeRecipes = new List<DrinkRuleSO>(); // locked recipes
    public List<DrinkRuleSO> unlockedRecipes = new List<DrinkRuleSO>(); // unlocked recipes
    public List<ItemDataSO> GetUnlockedItems() => unlockedItems;
    public List<DrinkRuleSO> GetUnlockedRecipes() => unlockedRecipes;
    public List<DrinkRuleSO> GetActiveRecipes() => activeRecipes;
}
