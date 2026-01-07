using System.Collections.Generic;
using UnityEngine;

public class FriendshipManager : MonoBehaviour
{
    public static FriendshipManager Instance;

    private Dictionary<string, FriendshipSaveData> freindships = new Dictionary<string, FriendshipSaveData>();
    private Dictionary<string, NPCIdentitySO> npcLookup = new Dictionary<string, NPCIdentitySO>();

    private void Awake()
    {
        Instance = this;
        LoadAllNPCs();
    }

    void LoadAllNPCs()
    {
        var npcs = Resources.LoadAll<NPCIdentitySO>("ScriptableObjectsData/NPCs");

        foreach (var npc in npcs)
        {
            //Debug.Log($"[FriendshipManager] Loaded NPC: {npc.npcName} with ID: {npc.npcID}");
            npcLookup[npc.npcID] = npc;
        }
    }

    public Sprite GetSpriteFromNPC(string npcID, string emotion)
    {
        if (!npcLookup.TryGetValue(npcID, out var npc))
        {
            Debug.LogWarning($"[FriendshipManager] NPC with ID {npcID} not found.");
            return null;
        }
        var emotionData = npc.emotions.Find(e => e.emotion == emotion);
        if (emotionData == null)
        {
            Debug.LogWarning($"[FriendshipManager] Emotion '{emotion}' not found for NPC {npcID}.");
            return null;
        }
        return emotionData.emotionSprite;
    }

    public FriendshipSaveData Get(string npcID)
    {
        if(!freindships.TryGetValue(npcID, out var data))
        {
            data = new FriendshipSaveData(npcID, 0, 0);
            freindships[npcID] = data;
        }
        return data;
    }

    public void AddXP(string npcID, int amount)
    {
        var data = Get(npcID);

        data.xp += amount;
        data.xp = Mathf.Max(0, data.xp);

        //Debug.Log($"[FriendshipManager] Added {amount} XP to {npcID}. Total XP: {data.xp}");

        int newLevel = data.xp / 10;

        //Debug.Log($"[FriendshipManager] Calculated new level for {npcID}: {newLevel}");

        if (newLevel > data.level)
        {
            data.level = newLevel;
            //Debug.Log($"[FriendshipManager] {npcID} leveled up to level {data.level}!");
            HandleLevelUp(npcID, newLevel);
        }
        else if (newLevel < data.level)
        {
            data.level = newLevel;
            //Debug.Log($"[FriendshipManager] {npcID} leveled down to level {data.level}.");
        }
    }

    private void HandleLevelUp(string npcID, int newLevel)
    {
        if (!npcLookup.TryGetValue(npcID, out var npc))
            return;

        foreach (var reward in npc.friendshipRewards)
        {
            if (reward.requiredLevel == newLevel)
            {
                if(UnlockManager.Instance.IsRecipeUnlocked(reward.rewardedRecipe) == false && reward.rewardType == RewardType.Recipe)
                {
                    UnlockReward(reward);
                }
            }
        }
    }

    private void UnlockReward(FriendshipReward reward)
    {
        //Debug.Log($"[FriendshipManager] Unlocked friendship reward: {reward.rewardDescription}");
        UnlockManager.Instance.UnlockRecipe(reward.rewardedRecipe);
    }

    public List<FriendshipSaveData> GetSaveData()
    {
        //Debug.Log($"[FriendshipManager] Saving {freindships.Count} friendships.");
        return new List<FriendshipSaveData>(freindships.Values);
    }

    public void ApplySaveData(List<FriendshipSaveData> data)
    {
        freindships.Clear();
        foreach (var entry in data)
        {
            //Debug.Log($"[FriendshipManager] Loaded friendship data for NPC ID: {entry.npcID}, Level: {entry.level}, XP: {entry.xp}");
            freindships[entry.npcID] = entry;
        }
    }
}
