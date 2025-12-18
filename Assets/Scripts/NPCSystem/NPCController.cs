using UnityEngine;
using UnityEngine.AI;

public class NPCController : MonoBehaviour
{
    public NPCIdentitySO identity;
    private NavMeshAgent agent;
    private Chair targetChair;
    private NPCState state;

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
}

