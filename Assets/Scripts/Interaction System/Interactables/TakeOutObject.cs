using UnityEngine;

public class TakeOutObject : MonoBehaviour, IInteractable
{
    [SerializeField] private ItemDataSO itemToTakeOut;
    [SerializeField] private string prompt;

    public string GetInteractionPrompt()
    {
        int amount = (itemToTakeOut) ? ItemsInventory.Instance.GetAmount(itemToTakeOut.id) : 0;
        return $"{prompt} ({amount})";
    }

    public void Interact(PlayerInventory player)
    {
        if (player.HasItem()) return;
        if (!itemToTakeOut || !itemToTakeOut.itemPrefab) return;

        if (ItemsInventory.Instance.TryRemove(itemToTakeOut.id, 1))
        {
            var itemInstance = Instantiate(itemToTakeOut.itemPrefab);
            var pickup = itemInstance.GetComponent<PickupItem>();
            
            if(pickup) pickup.Initialize(itemToTakeOut);
            player.PickUp(itemToTakeOut, itemInstance);
        }
    }
}
