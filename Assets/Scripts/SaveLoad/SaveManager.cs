using JetBrains.Annotations;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;

    public int ActiveSaveSlot { get; private set; } = 0;

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetActiveSaveSlot(int slot)
    {
        ActiveSaveSlot = slot;
        Debug.Log($"Active save slot set to {slot}");
    }

    public void SaveGame()
    {
        GameSaveData saveData = new GameSaveData
        {
            currentMoney = CurrencyManager.Instance.CurrentMoney,
            unlocks = UnlockManager.Instance.GetSaveData()
        };
        SaveSystem.Save(saveData, ActiveSaveSlot);
    }

    public void LoadGame()
    {
        if (!SaveSystem.InfoExists(ActiveSaveSlot))
        {
            SaveSystem.CreateNewSlotInfo(ActiveSaveSlot, "New Save");
        }
        GameSaveData saveData = SaveSystem.Load(ActiveSaveSlot);
        UnlockManager.Instance.ApplySaveData(saveData.unlocks);
        CurrencyManager.Instance.SetMoney(saveData.currentMoney);
    }
}
