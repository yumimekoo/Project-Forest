using UnityEngine;
using UnityEngine.UIElements;

public class OverlayUI : MonoBehaviour
{
    public UIDocument overlayUI;
    private VisualElement 
        root,
        backgroundElement,
        hideInRoom,
        iconHeld;
    
    private Label 
        moneyLabel,
        timeLabel,
        dayLabel,
        weekLabel,
        nameHeld;

    private Button
        pauseButton,
        bookButton;

    private bool isNight = false;

    private void OnEnable()
    {
        if(CurrencyManager.Instance)
        {
            CurrencyManager.Instance.OnMoneyChanged += UpdateMoney;
            UpdateMoney(CurrencyManager.Instance.CurrentMoney);
        }
        
        if(!UIManager.Instance) return;
            UIManager.Instance.OnUIStateChanged += HandleState;
            
        if(!PlayerInventory.Instance) return;
            PlayerInventory.Instance.OnItemChanged += UpdateHeldItem;
            
        if(!TimeManager.Instance) return;
            TimeManager.Instance.OnNewDayStarted += UpdateDate;
            TimeManager.Instance.OnProgressChanged += UpdateTime;
            TimeManager.Instance.OnNightTriggered += HandleNightTrigger;
    }

    private void OnDisable()
    {
        if (CurrencyManager.Instance)
        {
            CurrencyManager.Instance.OnMoneyChanged -= UpdateMoney;
        }
        if(!UIManager.Instance) return;
            UIManager.Instance.OnUIStateChanged -= HandleState;
            
        if(!PlayerInventory.Instance) return;
            PlayerInventory.Instance.OnItemChanged -= UpdateHeldItem;
            
        if(!TimeManager.Instance) return;
            TimeManager.Instance.OnNewDayStarted -= UpdateDate;
            TimeManager.Instance.OnProgressChanged -= UpdateTime;
            TimeManager.Instance.OnNightTriggered -= HandleNightTrigger;
    }

    private void HandleNightTrigger()
    {
        isNight = true;
        SetBackground("bg-night");
    }

    private void Awake()
    {
        root = overlayUI.rootVisualElement;
        moneyLabel = root.Q<Label>("moneyLabel");
        backgroundElement = root.Q<VisualElement>("backgroundElement");
        timeLabel = root.Q<Label>("timeLabel");
        dayLabel = root.Q<Label>("dayLabel");
        weekLabel = root.Q<Label>("weekLabel");
        pauseButton = root.Q<Button>("pauseButton");
        bookButton = root.Q<Button>("bookButton");
        hideInRoom = root.Q<VisualElement>("hideInRoom");
        iconHeld = root.Q<VisualElement>("iconHeld");
        nameHeld = root.Q<Label>("nameHeld");
        
        pauseButton.clicked += OnPauseButtonClicked;
        bookButton.clicked += OnBookButtonClicked;
    }

    private void OnBookButtonClicked()
    {
        UIManager.Instance.RecipeBookButtonPressed();
    }

    private void OnPauseButtonClicked()
    {
        UIManager.Instance.PauseButtonPressed();
    }

    private void UpdateMoney(int currentMoney)
    {
        moneyLabel.text = $"${currentMoney}";
    }

    private void SetBackground(string className)
    {
        backgroundElement.RemoveFromClassList("bg-room");
        backgroundElement.RemoveFromClassList("bg-night");
        backgroundElement.RemoveFromClassList("bg-day");
        backgroundElement.AddToClassList(className);
        Debug.Log($"Set background to {className}");
    }

    private void HandleState(UIState state)
    {
        bool isPaused = state == UIState.Pause;
        bool showOverlay = state == UIState.Overlay || state == UIState.Pause;
        
        root.style.display = showOverlay ? DisplayStyle.Flex : DisplayStyle.None;
        if (!showOverlay) return;

        if (GameState.isInCafe)
            EnterCafeUI();
        else if (GameState.isInRoom)
            EnterRoomUI();

        ApplyPauseModifier(isPaused);
    }

    private void EnterCafeUI()
    {
        pauseButton.style.display = DisplayStyle.Flex;
        bookButton.style.display =   DisplayStyle.Flex;
        hideInRoom.style.display =  DisplayStyle.Flex;
        SetBackground(isNight ? "bg-night" : "bg-day");
        UpdateTime(TimeManager.Instance.GetDayProgress());
    }
    
    private void EnterRoomUI()
    {
        Debug.Log("Entered room UI");
        pauseButton.style.display = DisplayStyle.Flex;
        bookButton.style.display = DisplayStyle.None;
        hideInRoom.style.display = DisplayStyle.None;
        SetBackground("bg-room");
        timeLabel.text = "10:00";
        UpdateTime(TimeManager.Instance.GetDayProgress());
        UpdateDate();
        UpdateHeldItem();
        Debug.Log("Exited room UI");
    }
    
    private void ApplyPauseModifier(bool isPaused)
    {
        if(isPaused)
            SetBackground("bg-room");
        hideInRoom.style.display = isPaused ? DisplayStyle.None : DisplayStyle.Flex;
        root.style.opacity = isPaused ? 0.7f : 1f;
    }

    private void UpdateHeldItem()
    {
        if (!GameState.isInCafe)
        {
            iconHeld.style.display = DisplayStyle.None;
            nameHeld.style.display = DisplayStyle.None;
            return;
        }
        
        var inv = PlayerInventory.Instance;
        if(!inv || !inv.heldItem)
        {
            iconHeld.style.display = DisplayStyle.None;
            nameHeld.style.display = DisplayStyle.None;
            return;
        }
        
        iconHeld.style.display = DisplayStyle.Flex;
        nameHeld.style.display = DisplayStyle.Flex;
        nameHeld.text = inv.heldItem.name;
        iconHeld.style.backgroundImage = inv.heldItem.icon ? new StyleBackground(inv.heldItem.icon) : null;
    }

    private void UpdateDate()
    {
        var tm = TimeManager.Instance;
        if(GameState.isInCafe)
            SetBackground(isNight ? "bg-night" : "bg-day");
        dayLabel.text = $"{tm.currentWeekday}, {tm.currentDay}";
        weekLabel.text = $"Week {tm.currentWeek}";
    }

    private void UpdateTime(float progress)
    {
        if (GameState.isInRoom)
        {
            timeLabel.text = "10:00";
            return;
        }
        
        int minutes = Mathf.RoundToInt(
            Mathf.Lerp(10 * 60, 26 * 60, progress)
        );

        minutes = (minutes / 5) * 5;

        int hour = (minutes / 60) % 24;
        int minute = minutes % 60;

        timeLabel.text = $"{hour:00}:{minute:00}";
    }
}
