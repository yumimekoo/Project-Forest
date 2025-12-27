using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public enum NPCSpecies
{
    Bear,
    Fox,
    Owl,
    Deer,
    Raccoon,
    Bat,
}

public enum NPCDaytimeAvailability
{
    Day,
    Night
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

[System.Serializable]
public class NPCEmotion
{
    public string emotion;
    public Sprite emotionSprite;
}

[CreateAssetMenu(menuName = "Scriptable Objects/NPCs Identity")]
public class NPCIdentitySO : ScriptableObject
{
    public string npcID;
    // public int numericID;
    public string npcName;
    public NPCSpecies species;
    public Sprite portrait;
    public NPCDaytimeAvailability availability;
    public GameObject npcPrefab;

    [Header("Timers")]
    [Tooltip("Timers are specific, can represent the tolerance of diverse npcs")]
    public float timeToAcceptOrder;
    public float timeToGiveOrder;
    public float timeDrinking;

    [Header("Order Preferences")]
    public List<ItemDataSO> favDrinks;
    public List<ItemDataSO> dislikedIngredients;
    public List<ItemDataSO> likedIngredients;

    [Header("Friendship Rewards")]
    public List<FriendshipReward> friendshipRewards;

    [Header("Yarn")]
    public YarnProject dialogueProject;
    public string startNode;

    [Header("Emotions")]
    [Tooltip("'happy', 'default', 'angry', 'sad'")]
    public List<NPCEmotion> emotions;
}
