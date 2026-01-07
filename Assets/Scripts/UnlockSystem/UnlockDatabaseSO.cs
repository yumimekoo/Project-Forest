using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnlockDatabaseSO", menuName = "Scriptable Objects/UnlockDatabase")]
public class UnlockDatabaseSO : ScriptableObject
{
    [Header("Items")]
    [HideInInspector] public List<ItemDataSO> allItems;
    public List<ItemDataSO> unlockedItems = new List<ItemDataSO>();

    [Header("Recipes")]
    [HideInInspector] public List<DrinkRuleSO> allRecipes;
    [HideInInspector] public List<DrinkRuleSO> activeRecipes = new List<DrinkRuleSO>();
    public List<DrinkRuleSO> unlockedRecipes = new List<DrinkRuleSO>();

    [Header("Furniture")]
    [HideInInspector] public List<FurnitureSO> allFurniture;
    public List<FurnitureSO> unlockedFurniture = new List<FurnitureSO>();
    public List<ItemDataSO> GetUnlockedItems() => unlockedItems;
    public List<DrinkRuleSO> GetUnlockedRecipes() => unlockedRecipes;
    public List<DrinkRuleSO> GetActiveRecipes() => activeRecipes;
    public List<FurnitureSO> GetUnlockedFurniture() => unlockedFurniture;
}
