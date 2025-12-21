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
        NPCInteractionManager.Instance.StartInteraction(this);
    }
}

