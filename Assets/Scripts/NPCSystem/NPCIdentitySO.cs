using UnityEngine;

public enum NPCSpecies
{
    Bear,
    Fox,
    Shroom
}

public enum NPCState
{
    Idle,
    Walking,
    Sitting,
    WaitingForOrder,
    WaitingForDrink,
    Leaving,
    Drinking,
    InConversation
}

[CreateAssetMenu(menuName = "Scriptable Objects/NPCs Identity")]
public class NPCIdentitySO : ScriptableObject
{
    public string npcID;
    // public int numericID;
    public string npcName;
    public NPCSpecies species;
    public Sprite portrait;
    public GameObject npcPrefab;
}
