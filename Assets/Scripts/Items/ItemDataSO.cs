using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Scriptable Objects/Item")]
public class ItemDataSO : ScriptableObject
{
    public int id;
    public string itemName;
    public ItemType itemType;

    public GameObject contentVisualPrefab;
    //public Sprite icon;

    [Header("World Prefab for Instantiating the Item")]

    public GameObject itemPrefab;
}
