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

        var unlockedItems = UnlockManager.Instance.runtimeDatabase.GetUnlockedFridgeItems();

        FridgeUI.Instance.OpenFridge(unlockedItems, (ItemDataSO selectedItem) =>
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
