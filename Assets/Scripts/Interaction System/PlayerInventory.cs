using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance;
    public ItemDataSO heldItem;
    public Transform handSlot;
    public GameObject heldObjectInstance;

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
        heldObjectInstance = Instantiate(item.itemPrefab, handSlot);
        SetLayerRecursively(heldObjectInstance, LayerMask.NameToLayer("HeldItem"));
        heldObjectInstance.transform.localPosition = Vector3.zero;
        heldObjectInstance.transform.localRotation = Quaternion.identity;
        Debug.Log($"Picked up: {item.itemName}");
    }

    public void PickUpCup(ItemDataSO item, GameObject cup)
    {
        heldItem = item;
        heldObjectInstance = cup;
        heldObjectInstance.transform.SetParent(handSlot);
        SetLayerRecursively(heldObjectInstance, LayerMask.NameToLayer("HeldItem"));
        heldObjectInstance.transform.localPosition = Vector3.zero;
        heldObjectInstance.transform.localRotation = Quaternion.identity;
        Debug.Log($"Picked up: {item.itemName}");
    }

    private void SetLayerRecursively(GameObject obj, int newLayer)
    {
        obj.layer = newLayer;

        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }
    public void DropItem()
    {
        if (heldItem != null)
        {
            Debug.Log($"Dropped: {heldItem.itemName}");
            heldItem = null;
            if (heldObjectInstance != null)
                Destroy(heldObjectInstance);
        }
    }
}
