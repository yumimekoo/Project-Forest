using UnityEngine;

public class PickupItem : MonoBehaviour, IInteractable
{
    public ItemDataSO item;
    public GameObject selfObj;

    public string GetInteractionPrompt()
    {
        return $"Press 'E' to pick up {item.itemName}";
    }
    public void Interact(PlayerInventory player)
    {
        if(player.HasItem()) return;

        player.PickUp(item, selfObj);
    }

}
