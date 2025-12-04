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
            currentMoney = CurrencyManager.Instance.CurrentMoney
        };
        SaveSystem.Save(saveData);
    }

    public void LoadGame()
    {
        GameSaveData saveData = SaveSystem.Load();
        CurrencyManager.Instance.SetMoney(saveData.currentMoney);
    }
}
