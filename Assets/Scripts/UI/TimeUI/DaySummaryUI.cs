using UnityEngine;
using UnityEngine.UIElements;

public class DaySummaryUI : MonoBehaviour
{
    [SerializeField] private UIDocument daySummaryUI;

    private Label
        moneyEarnedLabel,
        dayLabel,
        weekdayLabel,
        weekLabel,
        totalLabel,
        rentDueToLabel,
        completedLabel,
        //successLabel,
        //failedLabel,
        friendshipLabel,
        friendshipXPLabel;

    private Button
        okButton,
        payRentButton;

    private void Awake()
    {
        var root = daySummaryUI.rootVisualElement;
        moneyEarnedLabel = root.Q<Label>("moneyEarnedLabel");
        dayLabel = root.Q<Label>("day");
        weekdayLabel = root.Q<Label>("weekday");
        weekLabel = root.Q<Label>("week");
        totalLabel = root.Q<Label>("totalLabel");
        rentDueToLabel = root.Q<Label>("acceptedLabel");
        completedLabel = root.Q<Label>("completedLabel");
        //successLabel = root.Q<Label>("successLabel");
        //failedLabel = root.Q<Label>("failedLabel");
        friendshipLabel = root.Q<Label>("friendshipLabel");
        friendshipXPLabel = root.Q<Label>("friendshipXPLabel");
        okButton = root.Q<Button>("okButton");
        payRentButton = root.Q<Button>("payRentButton");

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

    private void Show(DayStats stats, int day, Weekday weekday, int week, int nextRentPrice)
    {
        dayLabel.text = $"Day {day}";
        weekdayLabel.text = weekday.ToString();
        weekLabel.text = $"Week {week}";

        moneyEarnedLabel.text = $"Money earned: {stats.moneyEarned}";
        totalLabel.text = $"Customer Orders Accepted: {stats.ordersAcceped} / {stats.totalCustomers}";
        completedLabel.text = $"Orders completed: {stats.ordersCompleted} with Success/Failed rate of {stats.ordersSuccess} / {stats.ordersFailed}";
        int daysUntilSunday = ((int)Weekday.Sunday - (int)weekday + 7) % 7;
        rentDueToLabel.text = $"{nextRentPrice}$ due in: {daysUntilSunday}";

        bool isSunday = weekday == Weekday.Sunday;

        payRentButton.text = $"Pay Rent: {nextRentPrice}";

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
