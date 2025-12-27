using System.Collections.Generic;
using UnityEngine;

public class NPCPool : MonoBehaviour
{
    public static NPCPool Instance;

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
            var npcInstance = Instantiate(npc.npcPrefab);
            npcInstance.SetActive(false);
            Debug.Log($"NPC Pool: Created instance of {npc.npcName}");
            npcInstances[npc] = npcInstance;
            availableNPCs.Add(npc);
        }
    }

    public bool HasNPCs => availableNPCs.Count > 0;

    public GameObject GetNPC()
    {
        if(!HasNPCs)
            return null;

        int index = Random.Range(0, availableNPCs.Count);
        Debug.Log($"NPC Pool: Providing NPC at index {index}");
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
}
