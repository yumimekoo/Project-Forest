using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Cup : MonoBehaviour, IInteractable
{
    public List<ItemDataSO> contents = new List<ItemDataSO>();
    public ItemDataSO currentItemData; // current state
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
        // return a list of all contents in the cup
        if (contents.Count == 0)
        {
            return "Add contents to the Cup.";
        }
        else
        {
            string contentNames = $"{currentItemData.itemName} contains: ";
            foreach (var item in contents)
            {
                contentNames += item.itemName + ", ";
            }
            contentNames = contentNames.TrimEnd(',', ' ');
            return contentNames;
        }
    }

    public void Interact(PlayerInventory player)
    {
        if (player.HasItem())
        {
            AddIngredient(player.heldItem);
            string contentNames = "Cup contains: ";
            foreach (var item in contents)
            {
                contentNames += item.itemName + ", ";
            }
            contentNames = contentNames.TrimEnd(',', ' ');
            //Debug.Log(contentNames);
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
