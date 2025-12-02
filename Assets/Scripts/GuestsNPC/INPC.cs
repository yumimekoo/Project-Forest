using UnityEngine;

public interface INPC
{
    ItemDataSO currentOrder { get; }
    bool hasOrderRunning { get; }
    void GiveOrder(ItemDataSO item);
}
