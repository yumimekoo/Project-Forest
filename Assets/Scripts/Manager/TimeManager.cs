using System;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }

    [Header("Time Settings")]
    public float dayDurationInSeconds;
    private float timeElapsed;

    private bool nightTriggered = false;
    private bool dayEnded = false;

    public event Action OnNightTriggered;
    public event Action OnDayEnded;
    public event Action<float> OnTimeChanged;
    public event Action<float> OnProgressChanged;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
    Instance = this;
    }

    private void FixedUpdate()
    {
        if (dayEnded)
            return;

        timeElapsed += Time.deltaTime;

        OnTimeChanged?.Invoke(GetTimeElapsed());
        OnProgressChanged?.Invoke(GetDayProgress());

        if (!nightTriggered && timeElapsed >= dayDurationInSeconds / 2)
        {
            nightTriggered = true;
            OnNightTriggered?.Invoke();
            GameState.isDay = false;
            GameState.isNight = true;
            Debug.Log("now is night");
        }

        if (!dayEnded && timeElapsed >= dayDurationInSeconds)
        {
            dayEnded = true;
            OnDayEnded?.Invoke();
            Debug.Log("day has ended");
        }
    }

    public float GetDayProgress() => Mathf.Clamp01(timeElapsed / dayDurationInSeconds);
    public float GetTimeElapsed() => timeElapsed;
}
