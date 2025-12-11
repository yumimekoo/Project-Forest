using UnityEngine;
[System.Serializable]
public class PlacedFurnitureData
{
    public int id;
    public int x;
    public int y;
    public int rotY;

    public PlacedFurnitureData(int id, int x, int y, int rotY)
    {
        this.id = id;
        this.x = x;
        this.y = y;
        this.rotY = rotY;
    }
}
