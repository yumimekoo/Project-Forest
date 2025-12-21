using System.Collections.Generic;
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

    public void GiveDrink(NPCController npc, List<ItemDataSO> contents, ItemDataSO givenDrink)
    {
        if(npc.currentOrder == null)
        {
            Debug.LogWarning("NPC has no current order.");
            return;
        }

        npc.ResolveOrder(givenDrink, contents);
    }
}
