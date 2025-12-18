using UnityEngine;

public class Chair : MonoBehaviour
{
    public bool isOccupied { get; private set; }
    public Transform seatPoint;

    public void Occupy()
    {
        isOccupied = true;
    }
    public void Free()
    {
        isOccupied = false;
    }
}
