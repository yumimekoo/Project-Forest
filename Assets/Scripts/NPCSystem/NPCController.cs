using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCController : MonoBehaviour, IInteractable
{
    public NPCIdentitySO identity;
    private NavMeshAgent agent;
    private Chair targetChair;
    private NPCState state;
    private float stateTimer;

    private float navmeshSpeed = 1.5f;
    private float navmeshAcceleration = 3f;


    public DrinkOrder currentOrder { get; private set; }
    public void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = navmeshSpeed;
        agent.acceleration = navmeshAcceleration;
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
        SetState(NPCState.Walking);
        agent.SetDestination(targetChair.seatPoint.position);
    }

    private void Update()
    {
        if(state == NPCState.Walking && !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            SitDown();
        }

        if(state == NPCState.Leaving && !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            Debug.Log($"{identity.npcName} has exited the cafe.");
            Destroy(gameObject);
        }

        if (stateTimer > 0f)
        {
            stateTimer -= GameTime.DeltaTime;
            if(stateTimer <= 0f)
            {
                OnStateTimerEnded();
            }
        }
    }

    private void OnStateTimerEnded()
    {
        switch(state)
        {
            case NPCState.Sitting:
                Leave("order was not Taken");
                break;
            case NPCState.WaitingForDrink:
                Leave("drink was not served");
                break;
            case NPCState.Drinking:
                Leave("finished drinking");
                break;
            default:
                break;
        }
    }

    private void SetState(NPCState newState, float timer = 0f)
    {
        state = newState;
        stateTimer = timer;
        Debug.Log($"{identity.npcName} state changed to {state}");
    }

    private void Leave(string reason)
    {
        Debug.Log($"{identity.npcName} leaving: {reason}");
        if(targetChair != null)
        {
            targetChair.Free();
        }
        SetState(NPCState.Leaving);
        agent.enabled = true;
        agent.SetDestination(ChairManager.Instance.GetExitPoint());
    }

    private void SitDown()
    {
        SetState(NPCState.Sitting, identity.timeToAcceptOrder); // 30 seconds to take order
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
        FriendshipManager.Instance.AddXP(identity.npcID, result.friendshipDelta);  
        SetState(NPCState.Drinking, identity.timeDrinking);
        currentOrder = null;
    }

    public void CreateOrder()
    {
        currentOrder = NPCOrderGenerator.GenerateOrder(identity);
        if(currentOrder != null)
        {
            Debug.Log($"{identity.npcName} has ordered: {currentOrder}");
            SetState(NPCState.WaitingForDrink, identity.timeToGiveOrder);
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
                return null;
        }
    }

    public void Interact(PlayerInventory player)
    {
        switch (state)
        {
            case NPCState.Sitting:
                NPCInteractionManager.Instance.StartInteraction(this);
                break;
            case NPCState.WaitingForDrink:
                if(!player.IsHoldingCup())
                {
                    Debug.Log("Player has no item to serve. (U need a Cup/Glass)");
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
                Debug.LogWarning("NPC is not in a state to interact.");
                break;
        }

    }
}

