using System.Collections.Generic;
using UnityEngine;

public class NPCPool : MonoBehaviour
{
    public static NPCPool Instance;

    [Header("NPC Prefabs")]
    public GameObject tutorialNPC;
    public GameObject baseNPC;

    [Header("NPCVisuals")] 
    public GameObject wolfVisual;
    public GameObject bearVisual;
    public GameObject foxVisual;
    public GameObject bunnyVisual;
    public GameObject batVisual;
    public GameObject deerVisual;
    
    private Dictionary<NPCIdentitySO, GameObject> npcInstances = new();
    private List<NPCIdentitySO> availableNPCs = new();

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        InitializePool();
    }

    private void InitializePool()
    {
        var npcs = Resources.LoadAll<NPCIdentitySO>("ScriptableObjectsData/NPCs");

        foreach (var npc in npcs)
        {
            var npcInstance = Instantiate(baseNPC);
            npcInstance.SetActive(false);

            ApplyNPCData(npc, npcInstance);
            
            npcInstances[npc] = npcInstance;
            availableNPCs.Add(npc);
        }
    }

    private void ApplyNPCData(NPCIdentitySO identity, GameObject npcInstance)
    {
        var controller = npcInstance.GetComponent<NPCController>();
        if(controller) controller.identity = identity;

        var view = npcInstance.GetComponent<NPCView>();
        if(!view) return;
        
        view.SetVisual(GetVisualPrefab(identity.species));
    }

    private GameObject GetVisualPrefab(NPCSpecies species)
    {
        return species switch
        {
            NPCSpecies.Bat => batVisual,
            NPCSpecies.Bear => bearVisual,
            NPCSpecies.Deer => deerVisual,
            NPCSpecies.Fox => foxVisual,
            NPCSpecies.Wolf => wolfVisual,
            NPCSpecies.Bunny => bunnyVisual,
            _ => null
        };
    }

    public bool HasNPCs => availableNPCs.Count > 0;

    public GameObject GetNPC()
    {
        if(!HasNPCs)
            return null;

        int index = Random.Range(0, availableNPCs.Count);
        //Debug.Log($"NPC Pool: Providing NPC at index {index}");
        var npcIdentity = availableNPCs[index];
        availableNPCs.RemoveAt(index);

        var npcInstance = npcInstances[npcIdentity];
        npcInstance.SetActive(true);
        return npcInstance;
    }

    public void ReturnNPC(NPCIdentitySO npcInstance)
    {
        if(!npcInstances.ContainsKey(npcInstance))
            return;

        var instance = npcInstances[npcInstance];
        instance.SetActive(false);

        availableNPCs.Add(npcInstance);
    }

    public bool AllNPCsReturned()
    {
        return availableNPCs.Count == npcInstances.Count;
    }

    public GameObject GetTutorialNPC()
    {
        var npcInstance = Instantiate(tutorialNPC);
        npcInstance.SetActive(true);
        return npcInstance;
    }
}
