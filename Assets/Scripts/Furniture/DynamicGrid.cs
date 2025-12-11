using UnityEngine;

public class DynamicGrid : MonoBehaviour
{
    [Header("Grid Size")]
    public int width = 10;
    public int height = 10;

    [Header("Cell Size")]
    public float cellSize = 1f;

    // ---------------------------------------------------------
    // ---- Coordinate Conversions
    // ---------------------------------------------------------

    public Vector3 GetWorldPosition(int x, int y)
    {
        return transform.position + new Vector3((x+0.5f) * cellSize, 0, (y+0.5f) * cellSize);
    }

    public Vector3 GetWorldPositionGizmos(int x, int y)
    {
        return transform.position + new Vector3(x * cellSize, 0, y * cellSize);
    }

    public Vector2Int WorldToGrid(Vector3 worldPos)
    {
        Vector3 local = worldPos - transform.position;

        int x = Mathf.FloorToInt(local.x / cellSize);
        int y = Mathf.FloorToInt(local.z / cellSize);

        return new Vector2Int(x, y);
    }

    public bool IsInsideGrid(int x, int y)
    {
        return x >= 0 && y >= 0 && x < width && y < height;
    }

    // ---------------------------------------------------------
    // ---- Scene View Visualization
    // ---------------------------------------------------------
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        for (int x = 0; x <= width; x++)
        {
            Vector3 start = GetWorldPositionGizmos(x, 0);
            Vector3 end = GetWorldPositionGizmos(x, height);
            Gizmos.DrawLine(start, end);
        }

        for (int y = 0; y <= height; y++)
        {
            Vector3 start = GetWorldPositionGizmos(0, y);
            Vector3 end = GetWorldPositionGizmos(width, y);
            Gizmos.DrawLine(start, end);
        }
    }
}
