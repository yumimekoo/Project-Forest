using System;
using UnityEngine;

public class BasicNPCTest : MonoBehaviour, IInteractable, INPC
{
    public NPCDataSO npcData;
    public ItemDataSO currentOrder { get; private set; }
    public bool hasOrderRunning { get; private set; }
    public event Action<int> OnCorrectOrderGiven;
    public event Action<int> OnWrongOrderGiven;

    private void OnEnable()
    {
        CurrencyManager.Instance.RegisterNPC(this);
    }

    private void OnDisable()
    {
        CurrencyManager.Instance.UnregisterNPC(this);
    }
    private void MakeOrder()
    {
        // For testing purposes, we can assign a dummy order
        hasOrderRunning = true;
        var unlockedRecipes = UnlockManager.Instance.runtimeDatabase.GetUnlockedRecipes();
        int randomIndex = UnityEngine.Random.Range(0, unlockedRecipes.Count);
        currentOrder = unlockedRecipes[randomIndex].resultingState;

        //Debug.Log($"NPC has made an order for: {currentOrder.itemName}");
    }

    public void GiveOrder(ItemDataSO item)
    {
        if (item == currentOrder)
        {
            OnCorrectOrderGiven?.Invoke(npcData.baseReward);
            Debug.Log("currect order! shanks!");
        }
        else
        {
            OnWrongOrderGiven?.Invoke(npcData.baseReward);
            Debug.Log("thats the wrong one :(");
        }
        hasOrderRunning = false;
    }

    public void Interact(PlayerInventory playerInventory)
    {
        if (playerInventory.HasItem() && hasOrderRunning)
        {
            ItemDataSO heldItem = playerInventory.heldItem;
            GiveOrder(heldItem);
            playerInventory.ClearItem();
        }
        else if (!hasOrderRunning)
        {
            MakeOrder();
            //Debug.Log("I would like to order a " + currentOrder.itemName);
        }
        else
        {
            Debug.Log("I already Ordered a" + currentOrder.itemName + " thanks :3");
        }
    }

    public string GetInteractionPrompt()
    {
        if (hasOrderRunning)
        {
            return $"Give Order: {currentOrder.itemName}";
        }
        else
        {
            return "Make Order";
        }
    }
}
