using UnityEngine;

public class TrashCan : MonoBehaviour, IInteractable
{
    public string GetInteractionPrompt()
    {
        return "Trash item";
    }

    public void Interact(PlayerInventory player)
    {
        if(player.HasItem())
        {
            player.DropItem();
            Debug.Log("Item trashed.");
        }
        else
        {
            Debug.Log("No item to trash.");
        }
    }
}
