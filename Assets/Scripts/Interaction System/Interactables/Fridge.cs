using UnityEngine;

public class Fridge : MonoBehaviour, IInteractable
{
    public string GetInteractionPrompt()
    {
        return "Open Fridge";
    }

    public void Interact(PlayerInventory player)
    {
        if (player.HasItem())
            return;

        var unlockedItems = UnlockManager.Instance.runtimeDatabase.GetUnlockedItems();
        var filteredItems = unlockedItems.FindAll(item => item.storageType == StorageType.Fridge && item.itemType == ItemType.Base);

        FridgeUI.Instance.OpenFridge(
            filteredItems,
            item => ItemsInventory.Instance.GetAmount(item.id) > 0,
            item => ItemsInventory.Instance.GetAmount(item.id),
            (ItemDataSO selectedItem) =>
        {
            if(selectedItem.itemPrefab == null)
            {
                Debug.LogWarning("Selected item has no prefab assigned.");
                return;
            }
            if (ItemsInventory.Instance.TryRemove(selectedItem.id, 1))
            {
                GameObject itemInstance = Instantiate(selectedItem.itemPrefab);
                var pickup = itemInstance.GetComponent<PickupItem>();
                pickup.Initialize(selectedItem);
                player.PickUp(selectedItem, itemInstance);
            }
        });
    }
}
