using UnityEngine;

public enum BuildCategory {     None,
    Furniture,
    Utility,
    Walls,
    Decorations,
    Lighting
}

public enum PlacementType { 
    Floor,
    Wall
}

[CreateAssetMenu(fileName = "FurnitureSO", menuName = "Scriptable Objects/FurnitureSO")]
public class FurnitureSO : ScriptableObject
{
    public string furnitureName;
    public string id;
    public int numericID;
    public BuildCategory buildCategory;
    public PlacementType placementType;
    public GameObject furniturePrefab;
    public GameObject furniturePreview;
    public Vector2Int size;
}
