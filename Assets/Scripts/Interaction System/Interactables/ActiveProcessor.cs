using System;
using UnityEngine;

public class ActiveProcessor : MonoBehaviour, IInteractable
{
    [Serializable]
    public class ProcessData
    {
        public ItemDataSO input;
        public ItemDataSO output;
        public int requiredInteractions;
    }

    [Header("Rules")] 
    [SerializeField] private ProcessData[] rules;

    [Header("Visuals (optional)")] 
    [SerializeField] private GameObject activeVisual;

    [SerializeField] private Transform itemPlacementPoint;

    [Header("Info")] 
    [SerializeField] private string machineName;
    [SerializeField] private string prompt;

    [Header("Soudns")]
    [SerializeField] private SoundSO placeSound;
    [SerializeField] private SoundSO interactionSound;
    
    
    private ProcessData activeRule;
    private int currentInteractions;
    private GameObject insertedInstance;

    public string GetInteractionPrompt()
    {
        if (activeRule == null) return prompt;
        return
            $"'E' to process {activeRule.input.itemName} ({currentInteractions} / {activeRule.requiredInteractions})";
    }

    public void Interact(PlayerInventory player)
    {
        if (activeRule == null)
        {
            if (!player.HasItem())
                return;
            var held = player.heldItem;
            var rule = FindRule(held);

            if (rule == null)
            {
                Debug.LogWarning($"No rule found for {held.itemName}");
                return;
            }
            
            InsertIngredient(player, rule);
            return;
        }

        if (player.HasItem())
        {
            Debug.Log("cannot while item in hand");
            return;
        }
        DoProcessingStep(player);
    }

    private ProcessData FindRule(ItemDataSO input)
    {
        if (!input || rules == null) return null;

        foreach (var r in rules)
        {
            if (r.input == input)
                return r;
        }

        return null;
    }

    private void InsertIngredient(PlayerInventory player, ProcessData rule)
    {
        activeRule = rule;
        currentInteractions = 0;

        if (player.heldObjectInstance)
        {
            AudioManager.Instance.PlayAt(placeSound, transform);
            insertedInstance = player.heldObjectInstance;
            player.RemoveReference();

            if (itemPlacementPoint)
            {
                insertedInstance.transform.SetParent(itemPlacementPoint);
                insertedInstance.transform.localPosition = Vector3.zero;
                insertedInstance.transform.localRotation = Quaternion.identity;
            }
        } else Debug.LogWarning("Player does not have an item to insert.");
        
        if(activeVisual) activeVisual.SetActive(true);
    }

    private void DoProcessingStep(PlayerInventory player)
    {
        if (player.HasItem())
            return;
        AudioManager.Instance.PlayAt(interactionSound, transform);
        currentInteractions++;

        if (currentInteractions >= activeRule.requiredInteractions) FinishProcessing(player);
    }
    
    private void FinishProcessing(PlayerInventory player)
    {
        if(activeVisual) activeVisual.SetActive(false);
        if(insertedInstance)
        {
            Destroy(insertedInstance);
            insertedInstance = null;
        }
        
        var outItem = activeRule.output;
        if (outItem && outItem.itemPrefab)
        {
            var go = Instantiate(outItem.itemPrefab);
            var pickup = go.GetComponent<PickupItem>();
            if(pickup) pickup.Initialize(outItem);
            
            player.PickUp(outItem, go);
        } else Debug.LogWarning("No output item found for this rule.");
        
        activeRule = null;
        currentInteractions = 0;
    }
}