using UnityEngine;

[CreateAssetMenu(fileName = "FurnitureSO", menuName = "Scriptable Objects/FurnitureSO")]
public class FurnitureSO : ScriptableObject
{
    public string furnitureName;
    public string id;
    public int numericID;
    public GameObject furniturePrefab;
}
