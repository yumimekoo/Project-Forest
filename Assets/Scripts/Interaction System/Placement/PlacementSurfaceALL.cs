using UnityEngine;
using System.Collections.Generic;


public class PlacementSurfaceALL : MonoBehaviour, IPlacableSurface
{
    public Transform placementPoint;

    public bool CanPlace(ItemDataSO item)
    {
        return true;
    }

    public Transform GetPlacementPoint()
    {
        return placementPoint;
    }
}
