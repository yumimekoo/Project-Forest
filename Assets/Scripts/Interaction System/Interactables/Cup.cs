using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum FillLevel {Half, Full }
public class Cup : MonoBehaviour, IInteractable
{
    public List<ItemDataSO> contents = new List<ItemDataSO>();
    public ItemDataSO currentItemData;
    public GameObject pickupCup;
    public ItemDataSO suspiciousDrinkData;
    
    [Header("Roots")]
    [SerializeField] private Transform visualRoot;
    
    [Header("Content Renderer")]
    [SerializeField] private GameObject cupFullModel;
    [SerializeField] private Renderer fullContentRenderer;
    [SerializeField] private GameObject cupHalfModel;
    [SerializeField] private Renderer halfContentRenderer;
    
    [Header("Standard content (color mode)")]
    [SerializeField] private string colorProperty = "_BaseColor";
    [SerializeField] private bool useMaterialPropertyBlock = true;
    
    [Header("Sounds")]
    [SerializeField] private SoundSO addToCupSound;
    
    private MaterialPropertyBlock mpb;
    private Renderer activeContentRenderer;
    private GameObject currentContentVisual;
    
    // private void Start()
    // {
    //     UpdateVisual(currentItemData);
    // }
    
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
            AudioManager.Instance.PlayAt(addToCupSound, transform);
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
        if (newItem == null) return;
        
        if (newItem.contentVisualPrefab != null)
        {
            ShowSpecial(newItem.contentVisualPrefab);
            return;
        }
        HideSpecial();
        
        SetFillLevel(newItem.fillLevel);
        
        ApplyColor(newItem.contentColor);
    }

    private void SetFillLevel(FillLevel level)
    {
        bool hasHalf = cupHalfModel && halfContentRenderer;

        bool full = level == FillLevel.Full || !hasHalf;

        if (cupFullModel) cupFullModel.SetActive(full);
        if (cupHalfModel) cupHalfModel.SetActive(!full);

        activeContentRenderer = full ? fullContentRenderer : halfContentRenderer;
    }

    private void ApplyColor(Color c)
    {
        if (!activeContentRenderer) return;

        if (!useMaterialPropertyBlock)
        {
            activeContentRenderer.material.SetColor(colorProperty, c);
            return;
        }

        mpb ??= new MaterialPropertyBlock();
        activeContentRenderer.GetPropertyBlock(mpb);
        mpb.SetColor(colorProperty, c);
        activeContentRenderer.SetPropertyBlock(mpb);
    }

    private void ShowSpecial(GameObject specialPrefab)
    {
        // Cup-Modelle ausblenden
        if (cupFullModel) cupFullModel.SetActive(false);
        if (cupHalfModel) cupHalfModel.SetActive(false);

        if (currentContentVisual)
            Destroy(currentContentVisual);

        if (visualRoot)
            currentContentVisual = Instantiate(specialPrefab, visualRoot);
    }

    private void HideSpecial()
    {
        if (currentContentVisual)
        {
            Destroy(currentContentVisual);
            currentContentVisual = null;
        }
        // Modelle werden beim SetFillLevel wieder gesetzt
    }

    // public void UpdateVisual(ItemDataSO newItem)
    // {
    //     if (newItem == null) return;
    //
    //     bool hasSpecial = newItem.contentVisualPrefab != null;
    //
    //     if (hasSpecial)
    //     {
    //         // Cup weg, Special rein
    //         SetCupVisible(false);
    //
    //         if (currentContentVisual != null)
    //             Destroy(currentContentVisual);
    //
    //         currentContentVisual = Instantiate(newItem.contentVisualPrefab, visualRoot);
    //         return;
    //     }
    //
    //     // Normal: Special weg, Cup an
    //     if (currentContentVisual != null)
    //     {
    //         Destroy(currentContentVisual);
    //         currentContentVisual = null;
    //     }
    //
    //     SetCupVisible(true);
    //
    //     if (contentRenderer == null) return;
    //     ApplyColor(newItem.contentColor);
    // }
    //
    // private void SetCupVisible(bool visible)
    // {
    //     if (cupModelRoot != null)
    //         cupModelRoot.SetActive(visible);
    // }
    //
    // private void ApplyColor(Color c)
    // {
    //     if (useMaterialPropertyBlock)
    //     {
    //         mpb ??= new MaterialPropertyBlock();
    //         contentRenderer.GetPropertyBlock(mpb);
    //         mpb.SetColor(colorProperty, c);
    //         contentRenderer.SetPropertyBlock(mpb);
    //     }
    //     else
    //     {
    //         contentRenderer.material.SetColor(colorProperty, c);
    //     }
    // }

}
