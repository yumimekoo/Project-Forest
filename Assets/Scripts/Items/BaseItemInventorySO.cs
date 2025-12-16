
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BaseItemInventorySO", menuName = "Scriptable Objects/BaseItemInventory")]
public class BaseItemInventorySO : ScriptableObject
{
    public List<ItemSaveData> startItems;
}
