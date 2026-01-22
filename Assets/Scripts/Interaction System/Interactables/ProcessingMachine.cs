using System.Collections;
using UnityEngine;

public class ProcessingMachine : MonoBehaviour, IInteractable
{
    [Header("Slots")]
    [SerializeField] private Transform cupPlacementPoint;
    
    [Header("Process Settings")]
    [SerializeField] private float processingTime;
    [SerializeField] private ItemDataSO ingredientToAdd;
    
    [Header("Visuals")]
    [SerializeField] private GameObject processingVisual;
    [SerializeField] private GameObject readyVisual;
    
    [Header("Prompts")]
    [SerializeField] private string promptPlaceCup;
    [SerializeField] private string promptProcessing;
    [SerializeField] private string promptReady;

    private bool isProcessing;
    private bool cupReady;
    private Cup cupInMachine;
    
    public string GetInteractionPrompt()
    {
        if(!cupInMachine) return promptPlaceCup;
        if(isProcessing) return promptProcessing;
        return cupReady ? promptReady : promptProcessing;
    }

    public void Interact(PlayerInventory player)
    {
        if (isProcessing) return;

        if (player.HasItem() && player.heldObjectInstance &&
            player.heldObjectInstance.TryGetComponent<Cup>(out Cup cup))
        {
            InsertCup(player, cup);
            return;
        }

        if (!player.HasItem() && cupInMachine)
        {
            ReturnCup(player);
            return;
        }
    }

    private void InsertCup(PlayerInventory player, Cup cup)
    {
        if (cupInMachine) return;
        
        cupInMachine = cup;
        cupReady = false;
        
        cupInMachine.transform.SetParent(cupPlacementPoint);
        cupInMachine.transform.localPosition = Vector3.zero;
        cupInMachine.transform.localRotation = Quaternion.identity;
        player.RemoveReference();

        StartCoroutine(ProcessCup());
    }

    private void ReturnCup(PlayerInventory player)
    {
        cupReady = false;
        
        player.PickUp(cupInMachine.currentItemData, cupInMachine.gameObject);
        cupInMachine = null;
        
        if(readyVisual) readyVisual.SetActive(false);
    }

    private IEnumerator ProcessCup()
    {
        isProcessing = true;
        if(processingVisual) processingVisual.SetActive(true);
        if(readyVisual) readyVisual.SetActive(false);
        
        yield return new WaitForSeconds(processingTime);
        
        if(cupInMachine && ingredientToAdd) cupInMachine.AddIngredient(ingredientToAdd);
        
        isProcessing = false;
        cupReady = true;
        
        if(processingVisual) processingVisual.SetActive(false);
        if(readyVisual) readyVisual.SetActive(true);
    }
}
