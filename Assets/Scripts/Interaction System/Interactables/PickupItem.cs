using UnityEngine;

public class PickupItem : MonoBehaviour, IInteractable
{
    public ItemDataSO item;

    public void Initialize(ItemDataSO newItem)
    {
        item = newItem;

        SetupVisuals();
    }

    private void SetupVisuals()
    {
        if (item.contentVisualPrefab == null)
            return;

        Transform visualsChild = transform.Find("visuals");
        if (visualsChild == null)
        {
            Debug.LogWarning($"No 'Visuals' child found on {gameObject.name}");
            return;
        }

        foreach (Transform child in visualsChild)
        {
            Destroy(child.gameObject);
        }

        GameObject newVisual = Instantiate(item.contentVisualPrefab, visualsChild);
        newVisual.name = item.contentVisualPrefab.name;
    }

    public string GetInteractionPrompt()
    {
        return $"Pick up {item.itemName}";
    }
    public void Interact(PlayerInventory player)
    {
        if(player.HasItem()) return;

        player.PickUp(item, gameObject);
    }

}
