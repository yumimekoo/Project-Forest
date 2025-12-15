
using System.Collections.Generic;
using UnityEngine;

public class UnlockerDEVELOPMENT : MonoBehaviour, IInteractable
{
    private int counter = 1;
    private List<ItemDataSO> items;

    public void Start()
    {
        items = new List<ItemDataSO>(Resources.LoadAll<ItemDataSO>("ScriptableObjectsData/ItemData/Base"));
    }
    public string GetInteractionPrompt()
    {
        return $"Unlock item with ID: {counter}";
    }

    public void Interact(PlayerInventory player)
    {
        UnlockManager.Instance.UnlockItem(items.Find(item => item.id == counter));
        counter++;
    }

}
