using UnityEngine;
using UnityEngine.UIElements;

public class DaySummaryUI : MonoBehaviour
{
    [SerializeField] private UIDocument daySummaryUI;
    [SerializeField] private VisualTreeAsset unlockedTemplate;

    private Label
        moneyEarnedLabel,
        dayLabel,
        weekdayLabel,
        weekLabel,
        totalLabel,
        rentDueToLabel,
        rentPayPrice,
        completedLabel,
        successLabel,
        failedLabel;

    private Button
        okButton,
        payRentButton;
    
    private VisualElement 
        unlockContainer;

    private void Awake()
    {
        var root = daySummaryUI.rootVisualElement;
        moneyEarnedLabel = root.Q<Label>("moneyEarnedLabel");
        dayLabel = root.Q<Label>("day");
        weekdayLabel = root.Q<Label>("weekday");
        weekLabel = root.Q<Label>("week");
        totalLabel = root.Q<Label>("totalLabel");
        rentDueToLabel = root.Q<Label>("rentDueToLabel");
        rentPayPrice = root.Q<Label>("rentPayPrice");
        successLabel = root.Q<Label>("successLabel");
        failedLabel = root.Q<Label>("failedLabel");
        completedLabel = root.Q<Label>("completedLabel");
        okButton = root.Q<Button>("okButton");
        payRentButton = root.Q<Button>("payRentButton");
        unlockContainer = root.Q<VisualElement>("unlockContainer");

        root.style.display = DisplayStyle.None;

        okButton.clicked += OnOkClicked;
        payRentButton.clicked += OnPayRentClicked;
    }

    private void OnEnable()
    {
        TimeManager.Instance.OnDaySummaryReady += Show;
    }
    private void OnDisable()
    {
        TimeManager.Instance.OnDaySummaryReady -= Show;
    }

    private void Show(DayStats stats, int day, Weekday weekday, int week, int nextRentPrice, UnlockReport report)
    {
        GameState.isInMenu = true;
        dayLabel.text = $"{day}, ";
        weekdayLabel.text = weekday.ToString();
        weekLabel.text = $"Week {week}";

        moneyEarnedLabel.text = $"{stats.moneyEarned}";
        totalLabel.text = $"{stats.ordersAcceped}/{stats.totalCustomers}";
        successLabel.text = $"{stats.ordersSuccess}";
        failedLabel.text = $"{stats.ordersFailed}";
        completedLabel.text = $"{stats.ordersCompleted}: ";
        int daysUntilSunday = ((int)Weekday.Sunday - (int)weekday + 7) % 7;
        rentPayPrice.text = $"{nextRentPrice}";
        rentDueToLabel.text = $"in {daysUntilSunday} days";

        bool isSunday = weekday == Weekday.Sunday;

        payRentButton.text = $"Pay {nextRentPrice}";
        
        unlockContainer.Clear();

        foreach (var item in report.unlockedItems)
        {
            var itemElement = unlockedTemplate.Instantiate();
            itemElement.Q<Label>("nameLabel").text = item.name;
            itemElement.Q<VisualElement>("itemIcon").style.backgroundImage = item.icon ? new StyleBackground(item.icon) : null;
            unlockContainer.Add(itemElement);
        }

        foreach (var furniture in report.unlockedFurniture)
        {
            var furnitureElement = unlockedTemplate.Instantiate();
            furnitureElement.Q<Label>("nameLabel").text = furniture.name;
            furnitureElement.Q<VisualElement>("itemIcon").style.backgroundImage = furniture.icon ? new StyleBackground(furniture.icon) : null;
            unlockContainer.Add(furnitureElement);
        }

        okButton.style.display = isSunday ? DisplayStyle.None : DisplayStyle.Flex;
        payRentButton.style.display = isSunday ? DisplayStyle.Flex : DisplayStyle.None;

        daySummaryUI.rootVisualElement.style.display = DisplayStyle.Flex;
        Time.timeScale = 0f;
    }

    private void OnOkClicked()
    {
        Close();
        TimeManager.Instance.ConfirmEndOfDay();
    }

    private void OnPayRentClicked()
    {
        Close();
        TimeManager.Instance.PayRent();
    }

    private void Close()
    {
        daySummaryUI.rootVisualElement.style.display= DisplayStyle.None;
        Time.timeScale = 1f;
    }
}
