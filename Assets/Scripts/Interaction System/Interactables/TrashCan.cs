using UnityEngine;

public class TrashCan : MonoBehaviour, IInteractable
{
    [SerializeField] SoundSO trashSound;
    public string GetInteractionPrompt()
    {
        return "Trash item";
    }

    public void Interact(PlayerInventory player)
    {
        if(player.HasItem())
        {
            AudioManager.Instance.PlayAt(trashSound, transform);
            player.ClearItem();
            //Debug.Log("Item trashed.");
        }
        else
        {
            //Debug.Log("No item to trash.");
        }
    }
}
