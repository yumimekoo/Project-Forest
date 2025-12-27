using UnityEngine;
using UnityEngine.UI;

public class YarnUIController : MonoBehaviour
{
    public static YarnUIController Instance;
    public Image spriteHolder;

    private void Awake()
    {
        Instance = this;
    }
    public void SetEmotion(string npcID, string emotion)
    {
        var sprite = FriendshipManager.Instance.GetSpriteFromNPC(npcID, emotion);
        spriteHolder.sprite = sprite;
    }
}
