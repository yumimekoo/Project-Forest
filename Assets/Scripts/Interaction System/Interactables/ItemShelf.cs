using UnityEngine;

public class ItemShelf : MonoBehaviour, IInteractable
{
    [Header("UI")] 
    [SerializeField] private string storageName;
    [SerializeField] private string prompt;
    
    [Header("Filter")]
    [SerializeField] private StorageType storageType;
    [SerializeField] private ItemType itemType = ItemType.Base;
    
    public string GetInteractionPrompt() => prompt;

    public void Interact(PlayerInventory player)
    {
        if (player.HasItem()) return;

        var unlockedItems = UnlockManager.Instance.runtimeDatabase.GetUnlockedItems();
        var filtered = unlockedItems.FindAll(item =>
            item && item.storageType == storageType && item.itemType == itemType);
        
        FridgeUI.Instance.OpenFridge(
            filtered,
            storageName,
            item => ItemsInventory.Instance.GetAmount(item.id) > 0,
            item => ItemsInventory.Instance.GetAmount(item.id),
            (ItemDataSO selectedItem) =>
            {
                if (!selectedItem || !selectedItem.itemPrefab)
                {
                    Debug.LogWarning("Selected item has no prefab assigned.");
                    return;
                }
                bool canTake =
                    selectedItem.isInfinite ||
                    ItemsInventory.Instance.TryRemove(selectedItem.id, 1);
                
                if (canTake)
                {
                    GameObject itemInstance = Instantiate(selectedItem.itemPrefab);
                    var pickup = itemInstance.GetComponent<PickupItem>();
                    if(pickup) pickup.Initialize(selectedItem);
                    
                    player.PickUp(selectedItem, itemInstance);
                }
            }
        );
    }
    
}
