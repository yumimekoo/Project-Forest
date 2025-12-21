using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCController : MonoBehaviour, IInteractable
{
    public NPCIdentitySO identity;
    private NavMeshAgent agent;
    private Chair targetChair;
    private NPCState state;

    public DrinkOrder currentOrder { get; private set; }
    public void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        FindChairAndGo();
    }

    private void FindChairAndGo()
    {
        targetChair = ChairManager.Instance.GetFreeChair();

        if(targetChair == null)
        {
            Debug.LogWarning($"No free chairs available for NPC: {identity.npcName}");
            return;
        }

        targetChair.Occupy();
        state = NPCState.Walking;
        agent.SetDestination(targetChair.seatPoint.position);
    }

    private void Update()
    {
        if(state == NPCState.Walking && !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            SitDown();
        }
    }

    private void SitDown()
    {
        state = NPCState.Sitting;
        agent.enabled = false;

        transform.position = targetChair.seatPoint.position;
        transform.rotation = targetChair.seatPoint.rotation;
    }

    public void ResolveOrder(ItemDataSO givenDrink, List<ItemDataSO> contents)
    {
        var result = NPCOrderResolver.Evaluate(identity, currentOrder, givenDrink, contents);
        if (result == null)
            return;

        Debug.Log($"{identity.npcName} order result: {result.outcome}, Money: {result.moneyDelta}, Friendship: {result.friendshipDelta}");

        // change here to drinking or something
        state = NPCState.Sitting;
        currentOrder = null;
    }

    public void CreateOrder()
    {
        currentOrder = NPCOrderGenerator.GenerateOrder(identity);
        if(currentOrder != null)
        {
            Debug.Log($"{identity.npcName} has ordered: {currentOrder}");
            state = NPCState.WaitingForDrink;
        }
    }

    public string GetInteractionPrompt()
    {
        switch (state)
        {
            case NPCState.Sitting:
                return $"Take order from {identity.npcName}";
            case NPCState.WaitingForDrink:
                if(currentOrder.requestedDrink != null)
                    return $"Serve {currentOrder.requestedDrink.itemName} to {identity.npcName}";
                else
                    return $"Serve drink to {identity.npcName}";
            case NPCState.Drinking:
                return $"start conversation with {identity.npcName}";
            default:
                return "";
        }
    }

    public void Interact(PlayerInventory player)
    {
        switch (state)
        {
            case NPCState.Sitting:
                NPCInteractionManager.Instance.StartInteraction(this);
                CreateOrder();
                break;
            case NPCState.WaitingForDrink:
                if(!player.IsHoldingCup())
                {
                    Debug.Log("Player has no item to serve.");
                    break;
                } else if (player.IsHoldingCup())
                {
                    var cup = player.heldObjectInstance.GetComponent<Cup>();
                    NPCInteractionManager.Instance.GiveDrink(this, cup.GetContents(), player.heldItem);
                    player.ClearItem();
                    break;
                }
                Debug.LogWarning("something went wrong serving drink to NPC.");
                break;
            case NPCState.Drinking:
                Debug.Log($"Starting conversation with {identity.npcName}");
                // Implement conversation logic here
                break;
            default:
                break;
        }

    }
}

