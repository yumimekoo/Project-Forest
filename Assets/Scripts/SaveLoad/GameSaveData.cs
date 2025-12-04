using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameSaveData
{
    public int currentMoney;
    public UnlockSaveData unlocks = new();
}
