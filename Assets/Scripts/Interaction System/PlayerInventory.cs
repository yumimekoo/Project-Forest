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

    public void PickUp(ItemDataSO item, GameObject obj)
    {
        heldItem = item;
        heldObjectInstance = obj;
        heldObjectInstance.transform.SetParent(handSlot);
        SetLayer(heldObjectInstance, LayerMask.NameToLayer("HeldItem"));
        heldObjectInstance.transform.localPosition = Vector3.zero;
        heldObjectInstance.transform.localRotation = Quaternion.identity;
        //Debug.Log($"Picked up: {item.itemName}");
    }

    public void PlaceDown(IPlacableSurface surface)
    {
        Transform placementPoint = surface.GetPlacementPoint();
        SetLayer(heldObjectInstance, LayerMask.NameToLayer("Interactables"));
        heldObjectInstance.transform.SetParent(placementPoint);
        heldObjectInstance.transform.localPosition = Vector3.zero;
        heldObjectInstance.transform.localRotation = Quaternion.identity;
        //Debug.Log($"Placed down: {heldItem.itemName}");
        heldItem = null;
        heldObjectInstance = null;
    }
    private void SetLayer(GameObject obj, int newLayer)
    {
        obj.layer = newLayer;
    }

    public void RemoveReference()
    {
        heldItem = null;
        heldObjectInstance = null;
    }

    public void ClearItem()
    {
        if (heldItem != null)
        {
            //Debug.Log($"Item: {heldItem.itemName} cleared.");
            heldItem = null;
            if (heldObjectInstance != null)
                Destroy(heldObjectInstance);
        }
    }
}
