using UnityEngine;
using UnityEngine.UIElements;

public class OverlayUI : MonoBehaviour
{
    public UIDocument overlayUI;
    private Label 
        moneyLabel;

    private void OnEnable()
    {
        var root = overlayUI.rootVisualElement;
        moneyLabel = root.Q<Label>("moneyLabel");

        if(CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.OnMoneyChanged += UpdateMoney;
            UpdateMoney(CurrencyManager.Instance.CurrentMoney);
        }
    }

    private void OnDisable()
    {
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.OnMoneyChanged -= UpdateMoney;
        }
    }

    private void UpdateMoney(int currentMoney)
    {
        moneyLabel.text = $"${currentMoney}";
    }
}
