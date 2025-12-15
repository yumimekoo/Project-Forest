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
        var filteredItems = unlockedItems.FindAll(item => item.id == 30); // here with itemType then? or StorageType enum

        FridgeUI.Instance.OpenFridge(filteredItems, (ItemDataSO selectedItem) =>
        {
            if(selectedItem.itemPrefab == null)
            {
                Debug.LogWarning("Selected item has no prefab assigned.");
                return;
            }
            GameObject itemInstance = Instantiate(selectedItem.itemPrefab);
            player.PickUp(selectedItem, itemInstance);
        });
    }
}
