using System.IO;
using UnityEngine;

public static class SaveSystem
{
    private static readonly string savePath = 
        Path.Combine(Application.persistentDataPath, "idontgive.eeffoc");
    private static readonly string KEY = "JonathernDerStinkerhihi";

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

    //public static void Save(GameSaveData saveData)
    //{
    //    string json = JsonUtility.ToJson(saveData);
    //    File.WriteAllText(savePath, json);
    //    Debug.Log($"Game saved to {savePath}");

    //    Debug.Log(File.Exists(SaveSystem.savePath)); // sollte true sein
    //    string content = File.ReadAllText(SaveSystem.savePath);
    //    Debug.Log(content);
    //}

    //public static GameSaveData Load()
    //{
    //    if (!File.Exists(savePath))
    //    {
    //        Debug.LogWarning("No save file found. Creating New");
    //        return new GameSaveData();
    //    }
    //    string json = File.ReadAllText(savePath);
    //    Debug.Log($"Game loaded from {savePath}");
    //    return JsonUtility.FromJson<GameSaveData>(json);
    //}
    public static void Save(GameSaveData saveData)
    {
        string json = JsonUtility.ToJson(saveData);
        string encrypted = EncryptDecrypt(json);
        File.WriteAllText(savePath, encrypted);
        Debug.Log($"Game saved to {savePath}");

        Debug.Log(File.Exists(SaveSystem.savePath)); // sollte true sein
        string content = File.ReadAllText(SaveSystem.savePath);
        Debug.Log(content);
    }

    public static GameSaveData Load()
    {
        if (!File.Exists(savePath))
        {
            Debug.LogWarning("No save file found. Creating New");
            return new GameSaveData();
        }
        string encrypted = File.ReadAllText(savePath);
        string decrypted = EncryptDecrypt(encrypted);
        Debug.Log($"Game loaded from {savePath}");
        return JsonUtility.FromJson<GameSaveData>(decrypted);
    }

    public static void DeleteSave()
    {
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
            Debug.Log("Save file deleted!");
        }
        else
        {
            Debug.Log("No save file to delete.");
        }
    }

    public static bool SaveExists()
    {
        return File.Exists(savePath);
    }
}
