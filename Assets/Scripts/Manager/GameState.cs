using UnityEngine;

public static class GameState
{
    // --- Player Movement Control ---
    public static bool playerMovementAllowed = true;
    public static bool playerInteractionAllowed = true;

    // --- Game Time and Day-Night Cycle ---
    public static bool isNight = false;
    public static bool isDay = true;

    // --- Room States ---
    public static bool isInRoom = false;
    public static bool isInCafe = true;
    public static bool doorUnlocked = false;
}
