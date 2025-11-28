using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Cup : MonoBehaviour, IInteractable
{
    public List<ItemDataSO> contents = new List<ItemDataSO>();
    public ItemDataSO cupItem;
    public GameObject cup;
    public string GetInteractionPrompt()
    {
        return "Add ingedient to cup";
    }

    public void Interact(PlayerInventory player)
    {
        if (player.HasItem())
        {
            contents.Add(player.heldItem);
            Debug.Log($"Added {player.heldItem.itemName} to the cup.");
            Debug.Log("Cup now contains:");
            foreach (var item in contents)
            {
                Debug.Log(item.itemName);
            }
            player.DropItem();
        }
        else
        {
            player.PickUpCup(cupItem, cup);
        }
    }
}
