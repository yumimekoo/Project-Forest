using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class UnlockReport
{
    public List<ItemDataSO> unlockedItems = new();
    public List<FurnitureSO> unlockedFurniture = new();
    
    public bool HasAny => unlockedItems.Count > 0 || unlockedFurniture.Count > 0;
}
public class DayUnlocker : MonoBehaviour
{
    [SerializeField] private DayUnlockScheduleSO schedule;
    private UnlockReport report = new();
    private UnlockManager unlockManager;

    public UnlockReport ProcessUnlocksForDay(int day)
    {
        report = new UnlockReport();
        ApplyUnlocksForDay(day);
        return report;
    }

    private void ApplyUnlocksForDay(int day)
    {
        if (!schedule || !schedule.TryGetEntry(day, out var entry)) return;

        unlockManager = UnlockManager.Instance;
        
        if(!unlockManager)
            Debug.LogError("no UnlockManager found");
        
        foreach (var item in entry.items)
        {
            if (!item) continue;
            
            if(unlockManager.UnlockItem(item)) report.unlockedItems.Add(item);
            
            if(item.isInfinite) ItemsInventory.Instance.Add(item.id, 1);
        }
        
        foreach (var furniture in entry.furniture)
        {
            if (!furniture) continue;
            
            if(unlockManager.UnlockFurniture(furniture)) report.unlockedFurniture.Add(furniture);
        }
    }
}
