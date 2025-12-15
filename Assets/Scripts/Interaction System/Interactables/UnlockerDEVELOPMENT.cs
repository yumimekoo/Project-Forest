using UnityEngine;

public class UnlockerDEVELOPMENT : MonoBehaviour, IInteractable
{
    [SerializeField] ItemDataSO itemToUnlock;
    public string GetInteractionPrompt()
    {
        return $"Unlock {itemToUnlock.itemName}";
    }

    public void Interact(PlayerInventory player)
    {
        UnlockManager.Instance.UnlockItem(itemToUnlock);
    }

}
