using System.Collections.Generic;
using UnityEngine;

public class FriendshipManager : MonoBehaviour
{
    public static FriendshipManager Instance;

    [Header("Friendship Level Settings")]
    [SerializeField] private int minLevel = 1;
    [SerializeField] private int maxLevel = 10;

    [Tooltip("XP needed to go from one level to the next (Level 1->2, 2->3, ...).")]
    [SerializeField] private int baseXpToNextLevel = 10;
    
    [SerializeField] private int xpIncreasePerLevel = 2;
    
    private Dictionary<string, FriendshipSaveData> freindships = new Dictionary<string, FriendshipSaveData>();
    private Dictionary<string, NPCIdentitySO> npcLookup = new Dictionary<string, NPCIdentitySO>();

    private void Awake()
    {
        Instance = this;
        LoadAllNPCs();
    }
    
    public int GetXpToNextLevel(int currentLevel)
    {
        if (currentLevel >= maxLevel) return 0;
        
        int xp = baseXpToNextLevel + (currentLevel - minLevel) * xpIncreasePerLevel;
        return Mathf.Max(1, xp);
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
    
    public FriendshipSaveData Get(string npcID)
    {
        if (!freindships.TryGetValue(npcID, out var data))
        {
            data = new FriendshipSaveData(npcID, minLevel, 0);
            freindships[npcID] = data;
        }
        
        data.level = Mathf.Clamp(data.level, minLevel, maxLevel);
        data.xp = Mathf.Max(0, data.xp);
        
        if (data.level >= maxLevel)
            data.xp = 0;

        return data;
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

    public void AddXP(string npcID, int amount)
    {
        var data = Get(npcID);
        
        if (data.level >= maxLevel)
        {
            data.level = maxLevel;
            data.xp = 0;
            return;
        }

        data.xp += amount;
        data.xp = Mathf.Max(0, data.xp);
        
        while (data.level < maxLevel)
        {
            int xpToNext = GetXpToNextLevel(data.level);
            if (xpToNext <= 0) break;

            if (data.xp >= xpToNext)
            {
                data.xp -= xpToNext; 
                data.level++;

                HandleLevelUp(npcID, data.level);

                if (data.level >= maxLevel)
                {
                    data.level = maxLevel;
                    data.xp = 0;
                    break;
                }
            }
            else
            {
                break;
            }
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
            entry.level = Mathf.Clamp(entry.level, minLevel, maxLevel);
            entry.xp = Mathf.Max(0, entry.xp);

            if (entry.level >= maxLevel)
                entry.xp = 0;

            freindships[entry.npcID] = entry;
        }
    }
}
