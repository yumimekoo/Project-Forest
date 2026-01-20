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
        
        if(!UIManager.Instance) Debug.LogError("UIManager not found!");
            UIManager.Instance.OnUIStateChanged += HandleState;
    }

    private void OnDisable()
    {
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.OnMoneyChanged -= UpdateMoney;
        }
        if(!UIManager.Instance) Debug.LogError("UIManager not found!");
            UIManager.Instance.OnUIStateChanged -= HandleState;
    }

    private void UpdateMoney(int currentMoney)
    {
        moneyLabel.text = $"${currentMoney}";
    }

    private void HandleState(UIState state)
    {
        switch (state)
        {
            case UIState.Overlay:
                Debug.LogWarning("OverlayUI::HandleState() called with Overlay state");
                if (GameState.isInCafe)
                {
                    Debug.Log("OverlayUI::HandleState() called with Cafe state");
                    // do something here
                    overlayUI.rootVisualElement.style.display = DisplayStyle.Flex;
                    return;
                }
                if (GameState.isInRoom)
                {
                    Debug.Log("OverlayUI::HandleState() called with Room state");
                    // do something here
                    overlayUI.rootVisualElement.style.display = DisplayStyle.None;
                    return;
                }
                break;
            case UIState.Tutorial:
                Debug.Log("OverlayUI::HandleState() called with Tutorial state");
                // todo
                // dol something here
                break;
            default:
                Debug.LogWarning("OverlayUI::HandleState() called with invalid state");
                overlayUI.rootVisualElement.style.display = DisplayStyle.None;
                break;
        }
    }
}
