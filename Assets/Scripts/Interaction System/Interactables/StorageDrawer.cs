using UnityEngine;

public class StorageDrawer : MonoBehaviour, IInteractable
{
        public string GetInteractionPrompt()
        {
            return "Open Storage";
        }

        public void Interact(PlayerInventory player)
        {
            if (player.HasItem())
                return;

            var unlockedItems = UnlockManager.Instance.runtimeDatabase.GetUnlockedStorageItems();

            FridgeUI.Instance.OpenFridge(unlockedItems, (ItemDataSO selectedItem) =>
            {
                if (selectedItem.itemPrefab == null)
                {
                    Debug.LogWarning("Selected item has no prefab assigned.");
                    return;
                }
                GameObject itemInstance = Instantiate(selectedItem.itemPrefab);
                player.PickUp(selectedItem, itemInstance);
            });
        }
    }
