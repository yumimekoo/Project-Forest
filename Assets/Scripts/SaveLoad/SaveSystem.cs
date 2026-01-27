using System.IO;
using UnityEngine;

public static class SaveSystem
{
    private static readonly string KEY = "JonathernDerStinkerhihi";

    private static string GetSaveFilePath(int slot)
    {
        switch (slot)
        {
            case 0:
                return Path.Combine(Application.persistentDataPath, "idontgive.eeffoc");
            case 1:
                return Path.Combine(Application.persistentDataPath, "igive.eeffoc");
            case 2:
                return Path.Combine(Application.persistentDataPath, "perchanceigive.eeffoc");
            default:
                Debug.LogWarning($"Invalid save slot {slot}. Defaulting to slot 0.");
                return Path.Combine(Application.persistentDataPath, "idontgive.eeffoc");
        }
    }

    private static string GetInfoFilePath(int slot)
    {
        switch (slot)
        {
            case 0:
                return Path.Combine(Application.persistentDataPath, "idontgive.iiffoc");
            case 1:
                return Path.Combine(Application.persistentDataPath, "igive.iiffoc");
            case 2:
                return Path.Combine(Application.persistentDataPath, "perchanceigive.iiffoc");
            default:
                Debug.LogWarning($"Invalid save-info slot {slot}. Defaulting to slot 0.");
                return Path.Combine(Application.persistentDataPath, "idontgive.iiffoc");
        }
    }

    private static string EncryptDecrypt(string data)
    {
        char[] key = KEY.ToCharArray();
        char[] output = new char[data.Length];

        for (int i = 0; i < data.Length; i++)
        {
            output[i] = (char) (data[i] ^ key[i % key.Length]);
        }

        return new string(output);
    }

    public static void Save(GameSaveData saveData, int slot)
    {
        string savePath = GetSaveFilePath(slot);
        string json = JsonUtility.ToJson(saveData);
        string encrypted = EncryptDecrypt(json);
        File.WriteAllText(savePath, encrypted);
        //Debug.Log($"Game saved to {savePath}");

        SaveSlotInfo info = new SaveSlotInfo();
        info.exists = true;
        info.slotNumber = slot;
        info.saveName = $"Save {slot+1}";      // Oder dynamisch vergeben
        info.createdDate = File.Exists(GetInfoFilePath(slot))
                            ? LoadSlotInfo(slot).createdDate   // beibehalten
                            : System.DateTime.Now.ToString();

        info.lastModifiedDate = System.DateTime.Now.ToString();
        info.currentDay = saveData.currentDay.day;
        info.currentMoney = saveData.currentMoney;
        info.currentWeek = saveData.currentDay.week.ToString();
        info.currentWeekday = ((Weekday)saveData.currentDay.weekDay).ToString();

        string infoJson = JsonUtility.ToJson(info, true);
        File.WriteAllText(GetInfoFilePath(slot), infoJson);

        //Debug.Log($"Slot info saved to {GetInfoFilePath(slot)}");
    }

    public static void CreateNewSlotInfo(int slot, string saveName)
    {
        SaveSlotInfo info = new SaveSlotInfo();
        info.exists = true;
        info.slotNumber = slot;
        info.saveName = saveName;
        info.createdDate = System.DateTime.Now.ToString();
        info.lastModifiedDate = info.createdDate;

        string json = JsonUtility.ToJson(info, true);
        File.WriteAllText(GetInfoFilePath(slot), json);

        //Debug.Log($"Slot info created at {GetInfoFilePath(slot)}");
    }

    public static bool InfoExists(int slot)
    {
        return File.Exists(GetInfoFilePath(slot));
    }

    public static SaveSlotInfo LoadSlotInfo(int slot)
    {
        string path = GetInfoFilePath(slot);

        if (!File.Exists(path))
            return null;

        string json = File.ReadAllText(path);
        return JsonUtility.FromJson<SaveSlotInfo>(json);
    }

    public static GameSaveData Load(int slot)
    {
        string savePath = GetSaveFilePath(slot);

        if (!File.Exists(savePath))
        {
            return new GameSaveData();
        }
        string encrypted = File.ReadAllText(savePath);
        string decrypted = EncryptDecrypt(encrypted);
        //Debug.Log($"Game loaded from {savePath}");
        return JsonUtility.FromJson<GameSaveData>(decrypted);
    }

    public static void DeleteSave(int slot)
    {
        string savePath = GetSaveFilePath(slot);

        if (File.Exists(savePath))
        {
            File.Delete(savePath);
        }

        if(InfoExists(slot))
        {
            File.Delete(GetInfoFilePath(slot));
        }

    }

    public static bool SaveExists(int slot)
    {
        return File.Exists(GetSaveFilePath(slot));
    }
}
