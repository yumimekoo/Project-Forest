using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance;
    public ItemDataSO heldItem;

    private void Awake()
    {
        Instance = this;
    }

    public bool HasItem()
    {
        return heldItem != null;
    }

    public void PickUpItem(ItemDataSO item)
    {
        heldItem = item;
        Debug.Log($"Picked up: {item.itemName}");
    }

    public void DropItem()
    {
        if (heldItem != null)
        {
            Debug.Log($"Dropped: {heldItem.itemName}");
            heldItem = null;
        }
    }
}
