using UnityEngine;

public static class GameState
{
    // --- Player Movement Control ---
    public static bool playerMovementAllowed = true;
    public static bool playerInteractionAllowed = true;
    public static bool isInConversation = false;

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
    
    // --- Menu --- 
    public static bool isInMenu = false;
    public static bool isInPauseMenu = false;
    public static bool isInStorage = false;

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
            Reset();
            SetToDefault();
            Debug.LogWarning("No GameStateSaveData found to apply.");
            return;
        }

        inTutorial = saveData.inTutorial;
        SetToDefault();
        //Debug.Log(GetSaveData());
    }

    public static void SetToDefault()
    {
        isInMenu = false;
        isInPauseMenu = false;
        isInStorage = false;
        isInBuildMode = false;
        playerMovementAllowed = true;
        playerInteractionAllowed = true;
        isInConversation = false;
    }
    
    public static void Reset()
    {
        inTutorial = true;
    }
}
