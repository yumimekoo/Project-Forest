using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Cup : MonoBehaviour, IInteractable
{
    public List<ItemDataSO> contents = new List<ItemDataSO>();
    public ItemDataSO currentItemData;
    public GameObject pickupCup;
    public Transform contentVisualRoot;
    public ItemDataSO suspiciousDrinkData;

    private GameObject currentContentVisual;

    private void Start()
    {
        UpdateVisual(currentItemData);
    }
    public string GetInteractionPrompt()
    {
        if (contents.Count == 0)
        {
            return "Add contents";
        }
        else
        {
            string contentNames = $"{currentItemData.itemName}";
            return contentNames;
        }
    }

    public void Interact(PlayerInventory player)
    {
        if (player.HasItem())
        {
            AddIngredient(player.heldItem);
            player.ClearItem();
        }
        else
        {
            player.PickUp(currentItemData, pickupCup);
        }
    }

    public void AddIngredient(ItemDataSO ingredient)
    {
        contents.Add(ingredient);
        //Debug.Log($"Ingredient {ingredient.itemName} added to the cup.");
        var recipes = UnlockManager.Instance.runtimeDatabase.GetUnlockedRecipes();
        foreach (var recipe in recipes)
        {
            if (recipe.requiredState == currentItemData && recipe.addedIngredient == ingredient)
            {
                currentItemData = recipe.resultingState;
                UpdateVisual(currentItemData);
                //Debug.Log($"Cup transformed to {currentItemData.itemName}.");
                return;
            }
        }
        //Debug.Log("Ingredient added, but no recipe rule matched.");
        currentItemData = suspiciousDrinkData;
        UpdateVisual(currentItemData);
    }

    public List<ItemDataSO> GetContents()
    {
        return contents;
    }

    public void UpdateVisual(ItemDataSO newItem)
    {
        if(currentContentVisual != null)
        {
            Destroy(currentContentVisual);
        }
        if (newItem.contentVisualPrefab != null)
        {
            currentContentVisual = Instantiate(newItem.contentVisualPrefab, contentVisualRoot);
        }
    }
}
