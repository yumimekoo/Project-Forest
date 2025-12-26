using System.Collections.Generic;
using UnityEngine;

public class ChairManager : MonoBehaviour
{
    public static ChairManager Instance;
    private List<Chair> chairs = new List<Chair>();
    public Transform exitPoint;
    private void Awake()
    {
        Instance = this;
        chairs.AddRange(FindObjectsByType<Chair>(FindObjectsSortMode.None));
    }

    public Chair GetFreeChair()
    {
        foreach (var chair in chairs)
        {
            if (!chair.isOccupied)
                return chair;
        }
        return null;
    }

    public Vector3 GetExitPoint()
    {
        return exitPoint.position;
    }
}
