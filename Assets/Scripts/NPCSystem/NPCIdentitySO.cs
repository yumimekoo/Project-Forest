using System.Collections.Generic;
using System.ComponentModel;
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

public enum RewardType
{
    Decoration,
    Recipe,
}
[System.Serializable]
public class FriendshipReward
{
    public string rewardDescription;
    public int requiredLevel;
    public RewardType rewardType;
    public DrinkRuleSO rewardedRecipe;
    public FurnitureSO rewardedDecoration;
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

    [Header("Timers")]
    [Description("Timers are specific, can represent the tolerance of diverse npcs")]
    public float timeToAcceptOrder;
    public float timeToGiveOrder;
    public float timeDrinking;

    [Header("Order Preferences")]
    public List<ItemDataSO> favDrinks;
    public List<ItemDataSO> dislikedIngredients;
    public List<ItemDataSO> likedIngredients;

    [Header("Friendship Rewards")]
    public List<FriendshipReward> friendshipRewards;
}
