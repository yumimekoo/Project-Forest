using UnityEngine;

public static class GameTime
{
    public static bool Paused { get; private set; } = false;

    public static float DeltaTime => Paused ? 0f : Time.deltaTime;

    public static void SetPaused(bool paused)
    {
        Paused = paused;
    }
}
