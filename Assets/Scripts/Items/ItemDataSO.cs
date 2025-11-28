using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Game/Item")]
public class ItemDataSO : ScriptableObject
{
    public string itemName;
    public ItemType itemType;
    public GameObject itemPrefab;
    //public Sprite icon;
}
