
using System.Collections.Generic;
using UnityEngine;

public class UnlockerDEVELOPMENT : MonoBehaviour, IInteractable
{
    [SerializeField] List<ItemDataSO> itemsList;
    [SerializeField] private List<DrinkRuleSO> rulesToActivate; 
    
    public string GetInteractionPrompt()
    {
        return "unlock";
    }

    public void Interact(PlayerInventory player)
    {

        foreach (var itme in itemsList)
        {
            UnlockManager.Instance.UnlockItem(itme);
            ItemsInventory.Instance.Add(itme.id, 10);
        }

        foreach (var rule in rulesToActivate)
        {
            UnlockManager.Instance.ActivateRecipe(rule);
        }

    }

}
