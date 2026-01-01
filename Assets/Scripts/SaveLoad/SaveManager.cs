using System.Collections;
using System.Collections.Generic;
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
            unlocks = UnlockManager.Instance.GetSaveData(),
            placedFurniture = FurniturePlacementManager.Instance.GetSaveData(),
            furnitureInventory = FurnitureInventory.Instance.GetSaveData(),
            itemInventory = ItemsInventory.Instance.GetSaveData(),
            friendships = FriendshipManager.Instance.GetSaveData(),
            currentDay = TimeManager.Instance.GetSaveData()
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
        ItemsInventory.Instance.ApplySaveData(saveData.itemInventory);
        FriendshipManager.Instance.ApplySaveData(saveData.friendships);
        FurniturePlacementManager.Instance.ApplySaveData(saveData.placedFurniture);
        FurnitureInventory.Instance.ApplySaveData(saveData.furnitureInventory);
        TimeManager.Instance.ApplySaveData(saveData.currentDay);

        StartCoroutine(StartRebuildAfterSceneLoaded(saveData.placedFurniture));
    }

    private IEnumerator StartRebuildAfterSceneLoaded(List<PlacedFurnitureData> items)
    {
        yield return null;
        BuildMode3D build = FindFirstObjectByType<BuildMode3D>();
        if (build == null)
        {
            Debug.LogError("BuildMode3D not found in scene.");
            yield break;
        }
        yield return StartCoroutine(build.RebuildFromSave(items));
    }
}
