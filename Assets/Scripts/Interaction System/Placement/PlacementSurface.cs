
using System.Collections.Generic;
using UnityEngine;

public class PlacementSurface : MonoBehaviour, IPlacableSurface
{
    public List<ItemType> allowedTypes;
    public Transform placementPoint;

    public bool CanPlace(ItemDataSO item)
    {
        return allowedTypes.Contains(item.itemType);
    }

    public Transform GetPlacementPoint()
    {
        return placementPoint;
    }
}
