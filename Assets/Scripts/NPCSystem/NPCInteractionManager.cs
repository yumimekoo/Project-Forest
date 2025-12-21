using UnityEngine;

public class NPCInteractionManager : MonoBehaviour
{
    public static NPCInteractionManager Instance;
    void Start()
    {
        Instance = this;
    }
    
    public void StartInteraction(NPCController npc)
    {
        Debug.Log($"Starting interaction with {npc.identity.npcName}");
        npc.CreateOrder();
        // Implement interaction logic here
    }
}
