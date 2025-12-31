using System;
using System.Collections.Generic;
using UnityEngine;

public class OrderManager : MonoBehaviour
{
    public static OrderManager Instance { get; private set; }

    private Dictionary<NPCIdentitySO, ItemDataSO> activeOrders = new Dictionary<NPCIdentitySO, ItemDataSO>();
    public IReadOnlyDictionary<NPCIdentitySO, ItemDataSO> ActiveOrders => activeOrders;

    public event Action OnOrderChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
    }

    public void AddOrder(NPCIdentitySO npc, ItemDataSO itemData)
    {
        activeOrders[npc] = itemData;
        OnOrderChanged?.Invoke();
    }

    public void RemoveOrder(NPCIdentitySO npc)
    {
        if (!activeOrders.Remove(npc))
            return;

        OnOrderChanged?.Invoke();
    }

    public bool HasOrder(NPCIdentitySO npc)
    {
        return activeOrders.ContainsKey(npc);
    }

    public bool TryGetOrder(NPCIdentitySO npc, out ItemDataSO itemData)
    {
        return activeOrders.TryGetValue(npc, out itemData);
    }
}
