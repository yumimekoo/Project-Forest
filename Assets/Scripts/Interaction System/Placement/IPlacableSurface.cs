using UnityEngine;

public interface IPlacableSurface
{
    bool CanPlace(ItemDataSO item);
    Transform GetPlacementPoint();
}
