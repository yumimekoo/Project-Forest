using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Yarn.Unity;

public class NPCController : MonoBehaviour, IInteractable
{
    public NPCIdentitySO identity;

    public SoundSO waitOrderSound;
    public SoundSO takeOrderSound;
    public SoundSO giveOrderSound;
    public SoundSO startConversationSound;
    
    private NPCOverheadUI overheadUI;
    private NavMeshAgent agent;
    private Chair targetChair;
    private NPCState state;
    private float stateTimer;
    private bool hasTalked = false;

    private float navmeshSpeed = 1.5f;
    private float navmeshAcceleration = 3f;

    public NPCState State => state;
    public float StateTimer => stateTimer;
    public bool HasTalked => hasTalked;

    public DrinkOrder currentOrder { get; private set; }
    public void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = navmeshSpeed;
        agent.acceleration = navmeshAcceleration;

    }
    public void FindChairAndGo()
    {
        targetChair = ChairManager.Instance.GetFreeChair();

        if(targetChair == null)
        {
            Debug.LogWarning($"No free chairs available for NPC: {identity.npcName}");
            return;
        }

        targetChair.Occupy();
        SetState(NPCState.Walking);
        agent.stoppingDistance = 0.2f;
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
            //Debug.Log($"{identity.npcName} has exited the cafe.");
            agent.enabled = false;
            NPCPool.Instance.ReturnNPC(identity);
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
 
    public void Reset()
    {
        hasTalked = false;
        currentOrder = null;
        stateTimer = 0f;
    }

    public void Initialize()
    {
        agent.enabled = true;
        
        if (NavMesh.SamplePosition(transform.position, out var hit, 2f, NavMesh.AllAreas))
        {
            agent.Warp(hit.position);
        }
        else
        {
            Debug.LogWarning($"NPC {name} konnte keinen NavMesh unter sich finden.");
        }
        
        FindChairAndGo();
        
        overheadUI = NPCUIManager.Instance.GetNPCUI();
        overheadUI.Init(this);
    }

    private void OnStateTimerEnded()
    {
        switch(state)
        {
            case NPCState.Sitting:
                Leave("order was not Taken");
                break;
            case NPCState.WaitingForDrink:
                OrderManager.Instance.RemoveOrder(identity);
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
        //Debug.Log($"{identity.npcName} state changed to {state}");

        overheadUI?.OnStateChanged(timer);
    }

    private void Leave(string reason)
    {
        transform.SetParent(null);
        //Debug.Log($"{identity.npcName} leaving: {reason}");
        if(targetChair)
        {
            targetChair.Free();
        }

        SetState(NPCState.Leaving);
        
        if (overheadUI)
        {
            overheadUI.ResetUI();
            NPCUIManager.Instance.ReturnUI(overheadUI);
            overheadUI = null;
        }
        
        agent.enabled = true;
        agent.stoppingDistance = 1f;
        agent.SetDestination(ChairManager.Instance.GetExitPoint());
    }

    private void SitDown()
    {
        SetState(NPCState.Sitting, identity.timeToAcceptOrder); // 30 seconds to take order
        agent.enabled = false;

        transform.SetParent(targetChair.seatPoint, worldPositionStays: false);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        AudioManager.Instance.PlayAt(waitOrderSound, transform);
        
        if (GameState.inTutorial && TutorialManager.Instance != null)
        {
            if (TutorialManager.Instance.currentStep == TutorialStep.FirstNPCSpawned)
            {
                TutorialManager.Instance.OnNPCSatDown();
                SetState(NPCState.Sitting, 9999f); // prevent leaving during tutorial
            }
        }
    }

    public void ResolveOrder(ItemDataSO givenDrink, List<ItemDataSO> contents)
    {
        var result = NPCOrderResolver.Evaluate(identity, currentOrder, givenDrink, contents);
        if (result == null)
            return;

        //Debug.Log($"{identity.npcName} order result: {result.outcome}, Money: {result.moneyDelta}, Friendship: {result.friendshipDelta}");
        // insert ui feedback here
        AudioManager.Instance.PlayAt(giveOrderSound, transform);
        OrderManager.Instance.RemoveOrder(identity);
        FriendshipManager.Instance.AddXP(identity.npcID, result.friendshipDelta);  
        CurrencyManager.Instance.AddMoney(result.moneyDelta);
        TimeManager.Instance.TrackFriendship(identity.npcName, result.friendshipDelta);
        TimeManager.Instance.TrackMoney(result.moneyDelta);
        TimeManager.Instance.TrackOrderCompleted();
        if(result.outcome != OrderOutcome.Wrong)
        {
            TimeManager.Instance.TrackOrderSuccsess();
        } 
        else
        {
            TimeManager.Instance.TrackOrderFailed();
        }
        SetState(NPCState.Drinking, identity.timeDrinking);
        currentOrder = null;

        if(GameState.inTutorial && TutorialManager.Instance != null)
        {
            if (TutorialManager.Instance.currentStep == TutorialStep.GiveCoffee)
            {
                TutorialManager.Instance.OnCoffeeGiven();
                if(targetChair)
                {
                    transform.SetParent(null);
                    targetChair.Free();
                }
                Destroy(gameObject);
            }
        }

    }

    public void CreateOrder()
    {
        currentOrder = NPCOrderGenerator.GenerateOrder(identity);

        AudioManager.Instance.PlayAt(takeOrderSound, transform);
        
        if(GameState.inTutorial && TutorialManager.Instance != null)
        {
            if (TutorialManager.Instance.currentStep == TutorialStep.TakeOrder)
            {
                currentOrder = NPCOrderGenerator.GenerateTutorialOrder();
                TutorialManager.Instance.OnOrderTaken();
            }
        }

        if (currentOrder != null)
        {
            OrderManager.Instance.AddOrder(identity, currentOrder.requestedDrink);
            TimeManager.Instance.TrackOrderAccepted();
            SetState(NPCState.WaitingForDrink, identity.timeToGiveOrder);
            if(GameState.inTutorial && TutorialManager.Instance != null)
            {
                SetState(NPCState.WaitingForDrink, 9999f);
            }
        }
    }

    public void StartConversation()
    {
        AudioManager.Instance.PlayAt(startConversationSound, transform);
        hasTalked = true;
        YarnManager.Instance.StartDialogue(identity.dialogueProject, identity.startNode);
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
                if (hasTalked || !identity.isTalkable)
                    return null;
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
                    //Debug.Log("Player has no item to serve. (U need a Cup/Glass)");
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
                if (hasTalked || !identity.isTalkable)
                {
                    Debug.LogWarning($"{identity.npcName} has already talked.");
                    break;
                }
                StartConversation();
                break;
            default:
                Debug.LogWarning("NPC is not in a state to interact.");
                break;
        }

    }
}

