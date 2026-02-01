
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Stack<T> where T : ScriptableObject
{
    public T item;
    public int amount = 1;
}

[CreateAssetMenu(fileName = "BaseItemInventorySO", menuName = "Scriptable Objects/BaseItemInventory")]
public class DefaultInventorySO : ScriptableObject
{
    public List<Stack<ItemDataSO>> defaultItems = new();
    public List<Stack<FurnitureSO>> defaultFurniture = new();
}
