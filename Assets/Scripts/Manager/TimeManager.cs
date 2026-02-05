using System;
using System.Collections.Generic;
using UnityEngine;

public enum Weekday
{
    Monday, 
    Tuesday, 
    Wednesday, 
    Thursday, 
    Friday, 
    Saturday, 
    Sunday
}

[Serializable]
public class DayStats
{
    public int moneyEarned;
    public int totalCustomers;
    public int ordersAcceped;
    public int ordersCompleted;
    public int ordersFailed;
    public int ordersSuccess;
    public Dictionary<string, int> freindshipPerNPCgained = new();

    public void ResetStats()
    {
        moneyEarned = 0;
        totalCustomers = 0;
        ordersAcceped = 0;
        ordersCompleted = 0;
        ordersFailed = 0;
        ordersSuccess = 0;
        freindshipPerNPCgained.Clear();
    }
}

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }

    [Header("Time Settings")]
    public float dayDurationInSeconds;
    private float timeElapsed = 0f;
    
    [SerializeField] private DayUnlocker dayUnlocker;
    private bool nightTriggered = false;

    [SerializeField] private GameObject doorGlow;
    
    [Header("Music")]
    public AudioClip nightMusic;
    public event Action OnNightTriggered;
    public event Action<float> OnTimeChanged;
    public event Action<float> OnProgressChanged;

    [Header("Current Day")]
    public int currentDay = 1;
    public int currentWeek = 1;
    public Weekday currentWeekday = Weekday.Monday;
    public DayStats todayStats = new();
    public int baseRentAmount = 100;

    public event Action <DayStats, int, Weekday, int, int, UnlockReport> OnDaySummaryReady;
    public event Action OnNewDayStarted;
    public event Action OnGameOver;
    public event Action OnTutorialComplete;
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
        if (GameState.dayEnded || GameState.inTutorial)
            return;

        timeElapsed += GameTime.DeltaTime;

        OnTimeChanged?.Invoke(GetTimeElapsed());
        OnProgressChanged?.Invoke(GetDayProgress());

        if (!nightTriggered && timeElapsed >= dayDurationInSeconds / 2)
        {
            nightTriggered = true;
            OnNightTriggered?.Invoke();
            GameState.isDay = false;
            GameState.isNight = true;
            AudioManager.Instance.CrossfadeMusic(nightMusic, 4f, true);
        }

        if (!GameState.dayEnded && timeElapsed >= dayDurationInSeconds)
        {
            GameState.dayEnded = true;
        }
    }

    public float GetDayProgress() => Mathf.Clamp01(timeElapsed / dayDurationInSeconds);
    public float GetTimeElapsed() => timeElapsed;

    public void StartNewDay()
    {
        if (GameState.isInRoom)
            return;
        
        ResetDay();
        StartDay();
    }

    public void TutorialComplete()
    {
        OnTutorialComplete?.Invoke();
    }

    public void InitializeFirstDay()
    {
        currentDay = 1;
        currentWeek = 1;
        currentWeekday = Weekday.Monday;
    }

    public void AdvanceDay()
    {
        if(GameState.inTutorial)
            return;
        //Debug.LogWarning("Advancing to next day");
        currentDay++;
        currentWeekday = (Weekday)(((int)currentWeekday + 1) % 7);
        if(currentWeekday == Weekday.Monday && currentDay > 1)
        {
            currentWeek++;
        }
    }

    private void StartDay()
    {
        OnNewDayStarted?.Invoke();
        doorGlow.SetActive(false);
    }

    private void ResetDay()
    {
        timeElapsed = 0f;
        nightTriggered = false;
        todayStats.ResetStats();
        GameState.isDay = true;
        GameState.isNight = false;
        GameState.dayEnded = false;
        GameState.doorUnlocked = false;
    }

    public void ShowDaySummary()
    {
        UnlockReport report = dayUnlocker.ProcessUnlocksForDay(currentDay);
        OnDaySummaryReady?.Invoke(todayStats, currentDay, currentWeekday, currentWeek, baseRentAmount*currentWeek, report);
    }

    public void ConfirmEndOfDay()
    {
        GameState.doorUnlocked = true;
        doorGlow.SetActive(true);
    }

    public void PayRent()
    {
        if(!CurrencyManager.Instance.SpendMoney(baseRentAmount * currentWeek))
        {
            OnGameOver?.Invoke();
            return;
        }

        GameState.doorUnlocked = true;

    }
    public void TrackMoney(int amount)
    {
        todayStats.moneyEarned += amount;
    }
    public void TrackCostumers()
    {
        todayStats.totalCustomers += 1;
    }
    public void TrackOrderAccepted()
    {
        todayStats.ordersAcceped += 1;
    }
    public void TrackOrderCompleted()
    {
        todayStats.ordersCompleted += 1;
    }
    public void TrackOrderFailed()
    {
        todayStats.ordersFailed += 1;
    }
    public void TrackOrderSuccsess()
    {
        todayStats.ordersSuccess += 1;
    }
    public void TrackFriendship(string npcName, int amount)
    {
        if (todayStats.freindshipPerNPCgained.ContainsKey(npcName))
        {
            todayStats.freindshipPerNPCgained[npcName] += amount;
        }
        else
        {
            todayStats.freindshipPerNPCgained[npcName] = amount;
        }
    }

    public DaySaveData GetSaveData()
    {
        return new DaySaveData(currentDay, (int)currentWeekday, currentWeek);
    }

    public void ApplySaveData(DaySaveData data)
    {
        if(data == null)
        {
            //Debug.LogWarning("No TimeManager Save Data to apply, INITIALIZING");
            InitializeFirstDay();
            return;
        }
        //Debug.LogWarning("Applying TimeManager Save Data");
        currentDay = data.day;
        currentWeekday = (Weekday)data.weekDay;
        currentWeek = data.week;
        StartNewDay();
    }

}
