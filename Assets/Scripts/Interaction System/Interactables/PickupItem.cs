using UnityEngine;

public class PickupItem : MonoBehaviour, IInteractable
{
    public ItemDataSO item;

    public string GetInteractionPrompt()
    {
        return $"Press 'E' to pick up {item.itemName}";
    }
    public void Interact(PlayerInventory player)
    {
        if(player.HasItem()) return;

        player.PickUpItem(item);
        Destroy(gameObject); // maybe noch pooling, but i dont think we need that tho
    }

}
