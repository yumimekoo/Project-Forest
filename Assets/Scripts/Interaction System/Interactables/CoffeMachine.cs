using System.Collections;
using UnityEditor;
using UnityEditor.XR;
using UnityEngine;

public class CoffeMachine : MonoBehaviour, IInteractable
{
    [Header("Slots")]
    public Transform cupPlacementPoint;
    public GameObject beanVisual;

    [Header("Settings")]
    public float brewingTime = 5f;
    public ItemDataSO ingredientToAdd;

    private bool isBrewing = false;
    private bool hasBeans = false;
    private Cup cupInMachine;

    public string GetInteractionPrompt()
    {
        if (cupInMachine == null)
        {
            return "Place a cup to brew coffee.";
        }
        else if (isBrewing)
        {
            return "Brewing coffee...";
        }
        else if (!hasBeans)
        {
            return "Add coffee beans to start brewing.";
        }
        else
        {
            return "Collect your brewed coffee.";
        }
    }

    public void Interact(PlayerInventory player)
    {
        if (isBrewing)
            return;

        if (player.HasItem() && player.heldItem.itemType == ItemType.CoffeeBean)
        {
            AddBeans(player);
            return;
        }

        if (player.HasItem() && player.heldObjectInstance.TryGetComponent<Cup>(out Cup cup))
        {
            InsertCup(player, cup);
            return;
        }

        if (!player.HasItem() && cupInMachine != null)
        {
            ReturnCup(player);
            return;
        }

        if (!player.HasItem())
        {
            Debug.Log("You need to place a cup and add coffee beans first.");
        }
    }

    private void AddBeans(PlayerInventory player)
    {
        if (hasBeans)
        {
            Debug.Log("Coffee beans are already added.");
            return;
        }
        beanVisual.SetActive(true);
        hasBeans = true;
        player.ClearItem();
        TryStartBrew();
    }

    private void InsertCup(PlayerInventory player, Cup cup)
    {
        if (cupInMachine != null)
        {
            Debug.Log("Cup is already in the machine.");
            return;
        }
        cupInMachine = cup;
        cup.transform.SetParent(cupPlacementPoint);
        cup.transform.localPosition = Vector3.zero;
        cup.transform.localRotation = Quaternion.identity;
        player.RemoveReference();
        TryStartBrew();
    }

    private void ReturnCup(PlayerInventory player)
    {
        if (isBrewing) return;

        player.PickUp(cupInMachine.currentItemData, cupInMachine.gameObject);
        cupInMachine = null;
    }

    private void TryStartBrew()
    {
        if (cupInMachine != null && hasBeans)
        {
            StartCoroutine(BrewCoffee());
        }
    }

    private IEnumerator BrewCoffee()
    {
        isBrewing = true;
        Debug.Log("Brewing coffee...");

        yield return new WaitForSeconds(brewingTime);

        cupInMachine.AddIngredient(ingredientToAdd);
        beanVisual.SetActive(false);
        Debug.Log("Coffee is ready! You can now collect your cup.");

        isBrewing = false;
        hasBeans = false;
    }
}
