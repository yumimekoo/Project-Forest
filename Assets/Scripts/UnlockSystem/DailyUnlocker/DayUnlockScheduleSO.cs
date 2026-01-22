using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DayUnlockScheduleSO", menuName = "Scriptable Objects/DayUnlockScheduleSO")]
public class DayUnlockScheduleSO : ScriptableObject
{
    public List<DayUnlockEntry> entries = new();
    
    public bool TryGetEntry(int day, out DayUnlockEntry entry)
    {
        entry = entries.Find(e => e.day == day);
        return entry != null;
    }
}

[System.Serializable]
public class DayUnlockEntry
{
    public int day;
    
    [Header("Unlock on this day")]
    public List<FurnitureSO> furniture;
    public List<ItemDataSO> items;
}
