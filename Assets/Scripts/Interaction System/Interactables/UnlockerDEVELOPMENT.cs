using UnityEngine;

public class UnlockerDEVELOPMENT : MonoBehaviour, IInteractable
{
    [SerializeField] ItemDataSO itemToUnlock;
    [SerializeField] ItemDataSO itemToUnlock2;
    public string GetInteractionPrompt()
    {
        return $"Unlock {itemToUnlock.itemName}";
    }

    public void Interact(PlayerInventory player)
    {
        UnlockManager.Instance.UnlockItem(itemToUnlock);
        UnlockManager.Instance.UnlockItem(itemToUnlock2);
    }

}
