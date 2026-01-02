using UnityEngine;

public static class GameState
{
    // --- Player Movement Control ---
    public static bool playerMovementAllowed = true;
    public static bool playerInteractionAllowed = true;

    // --- Game Time and Day-Night Cycle ---
    public static bool isNight = false;
    public static bool isDay = true;
    public static bool dayEnded = true;

    // --- Room States ---
    public static bool isInRoom = true;
    public static bool isInCafe = false;
    public static bool doorUnlocked = true;

    // --- Build Mode ---
    public static bool isInBuildMode = false;

    // --- Tutorial State ---
    public static bool inTutorial = true;

    public static GameStateSaveData GetSaveData()
    {
        return new GameStateSaveData(
            inTutorial
        );  
    }

    public static void ApplySaveData(GameStateSaveData saveData)
    {
        if(saveData == null)
        {
            Debug.LogWarning("No GameStateSaveData found to apply.");
            return;
        }

        inTutorial = saveData.inTutorial;

        Debug.Log(GetSaveData());
    }
}
