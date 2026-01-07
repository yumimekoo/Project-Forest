using UnityEngine;
using Yarn.Unity;

public class YarnCommands : MonoBehaviour
{

    [YarnCommand("friendship")]
    public static void IncreaseFriendship(string npcID, int amount)
    {
        //Debug.Log($"[YarnCommands] Increasing friendship for {npcID} by {amount}");
        FriendshipManager.Instance.AddXP(npcID, amount);
    }

    [YarnFunction("get_level")]
    public static int GetFriendshipLevel(string npcID)
    {
        return FriendshipManager.Instance.Get(npcID).level;
    }

    [YarnCommand("set_emotion")]
    public static void SetEmotion(string npcID, string emotion)
    {
        YarnUIController.Instance.SetEmotion(npcID, emotion);
    }
}
