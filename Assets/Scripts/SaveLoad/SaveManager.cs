using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        LoadGame();
    }

    public void SaveGame()
    {
        GameSaveData saveData = new GameSaveData
        {
            currentMoney = CurrencyManager.Instance.CurrentMoney,
            unlocks = UnlockManager.Instance.GetSaveData()
        };
        SaveSystem.Save(saveData);
    }

    public void LoadGame()
    {
        GameSaveData saveData = SaveSystem.Load();
        Debug.Log($"Loaded save data: {saveData.unlocks.unlockedFridgeItemIDs.Count} fridge, {saveData.unlocks.unlockedStorageItemIDs.Count} storage, {saveData.unlocks.unlockedRecipeIDs.Count} recipes");
        UnlockManager.Instance.ApplySaveData(saveData.unlocks);
        CurrencyManager.Instance.SetMoney(saveData.currentMoney);
    }
}
