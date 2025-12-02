using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NPCData", menuName = "NPC/NPC Data")]
public class NPCDataSO : ScriptableObject
{
    public string npcName;
    public Sprite npcPortrait;

    [Header("Dialogue Lines")]
    public string[] greetingLines;

    [Header("Preferred Orders")]
    public List<ItemDataSO> preferredOrders;

    [Header("Reward Settings")]
    public int baseReward;
}
