using UnityEngine;
using UnityEngine.UI;

public class YarnUIController : MonoBehaviour
{
    public static YarnUIController Instance;
    public Image spriteHolder;

    [SerializeField] private Image textBackground;
    [SerializeField] private Image optionsBackground;
    
    [SerializeField] private Sprite[] textLevelSprites = new Sprite[10];
    [SerializeField] private Sprite[] optionsLevelSprites = new Sprite[10];

    private void Awake()
    {
        Instance = this;
    }
    public void SetEmotion(string npcID, string emotion)
    {
        var sprite = FriendshipManager.Instance.GetSpriteFromNPC(npcID, emotion);
        spriteHolder.sprite = sprite;
    }

    public void ApplyNpcUI(string npcID)
    {
        int level = FriendshipManager.Instance.Get(npcID).level;
        if(textBackground) textBackground.sprite = textLevelSprites[level];
        if(optionsBackground) optionsBackground.sprite = optionsLevelSprites[level];
    }
}
