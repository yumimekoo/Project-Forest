using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class ShopUI : MonoBehaviour
{
    public UIDocument shopUI;
    public VisualTreeAsset shopItemTemplate;
    public VisualTreeAsset shopCategoryTabTemplate;
    public SoundSO menuOpenSound;
    public SoundSO menuCloseSound;
    public SoundSO hoverSound;
    public SoundSO clickSound;
    public SoundSO buySound;
    
    private VisualElement
        itemContainer,
        tabsContainer;
    private Button
        exitButton;
    private Label moneyLabel;
    private ShoppingCategory currentCategory;
    List<IShopItem> allShopItems;
    
    private bool inShop = false;

    private Dictionary<ShoppingCategory, Button> categoryButtons = new();

    private void Awake()
    {
        InitUI();
    }

    private void Start()
    {
        UpdateButtons();
    }

    private void OnEnable()
    {
        if (CurrencyManager.Instance != null)
            CurrencyManager.Instance.OnMoneyChanged += UpdateMoney;
        if (UIManager.Instance)
        {
            UIManager.Instance.OnButtonsUpdated += UpdateButtons;
            UIManager.Instance.OnUIStateChanged += HandleState;
            UIManager.Instance.OnEscapePressed += EscapePressed;
        }
    }

    private void OnDisable()
    {
        if (CurrencyManager.Instance != null)
            CurrencyManager.Instance.OnMoneyChanged -= UpdateMoney;
        if (UIManager.Instance)
        {
            UIManager.Instance.OnButtonsUpdated -= UpdateButtons;
            UIManager.Instance.OnUIStateChanged -= HandleState;
            UIManager.Instance.OnEscapePressed -= EscapePressed;
        }
    }

    private void HandleState(UIState state)
    {
        switch (state)
        {
            case UIState.Shop:
                ShowUI();
                break;
            case UIState.Tutorial:
                // later
            default:
                shopUI.rootVisualElement.style.display = DisplayStyle.None;
                break;
        }
        
    }
    private void InitUI()
    {
        var root = shopUI.rootVisualElement;
        itemContainer = root.Q<VisualElement>("itemContainer");
        tabsContainer = root.Q<VisualElement>("tabsContainer");
        exitButton = root.Q<Button>("exitButton");
        moneyLabel = root.Q<Label>("moneyLabel");

        if(CurrencyManager.Instance != null)
            moneyLabel.text = CurrencyManager.Instance.CurrentMoney.ToString();
        exitButton.clicked += OnExitButton;

        BuildShopItems();
        BuildCategoryTabs();

        foreach (var button in root.Query<Button>().ToList())
        {
            button.RegisterCallback<MouseEnterEvent>(_ =>
            {
                AudioManager.Instance.Play(hoverSound);
            });
        }

        ShowCategory(ShoppingCategory.Resources);
        HideUI();
    }

    private void ShowUI()
    {
        inShop = true;
        AudioManager.Instance.Play(menuOpenSound);
        BuildShopItems();
        ShowCategory(currentCategory);
        shopUI.rootVisualElement.style.display = DisplayStyle.Flex;
        GameState.playerInteractionAllowed = false;
        GameState.playerMovementAllowed = false;
        GameState.isInMenu = true;
    }

    private void HideUI()
    {
        inShop = false;
        shopUI.rootVisualElement.style.display = DisplayStyle.None;
        GameState.playerInteractionAllowed = true;
        GameState.playerMovementAllowed = true;
        GameState.isInMenu = false;
    }

    private void EscapePressed()
    {
        if(inShop) OnExitButton();
    }
    
    private void OnExitButton()
    { 
        AudioManager.Instance.Play(menuCloseSound);
        UIManager.Instance.ResetState();
        HideUI();

        if (GameState.inTutorial && TutorialManager.Instance)
        {
            TutorialManager.Instance.OnExitPressed();
        }
    }

    private void UpdateMoney(int newAmount)
    {
        moneyLabel.text = newAmount.ToString();
    }

    private void BuildShopItems()
    {
        allShopItems = new List<IShopItem>();

        foreach (var item in UnlockManager.Instance.runtimeDatabase.GetUnlockedItems())
            allShopItems.Add(new ShopItem_Item(item));

        foreach (var furniture in UnlockManager.Instance.runtimeDatabase.GetUnlockedFurniture())
            allShopItems.Add(new ShopItem_Furniture(furniture));
    }

    private void ShowCategory(ShoppingCategory category)
    {
        currentCategory = category;

        foreach (var kvp in categoryButtons)
        {
            kvp.Value.RemoveFromClassList("selected");
        }

        categoryButtons[category].AddToClassList("selected");

        itemContainer.Clear();
        foreach (var shopItem in allShopItems.Where(i => i.Category == category))
        {
            CreateEntry(shopItem);
        }

    }

    private void CreateEntry(IShopItem shopItem)
    {
        
        var entry = shopItemTemplate.Instantiate();

        var nameLabel = entry.Q<Label>("nameLabel");
        var priceLabel = entry.Q<Label>("priceLabel");
        var ownedAmountLabel = entry.Q<Label>("ownedAmountLabel");
        var buyAmountLabel = entry.Q<Label>("buyAmountLabel");
        var buyButton = entry.Q<Button>("buyButton");
        var totalPriceLabel = entry.Q<Label>("totalPrice");
        var iconElement = entry.Q<VisualElement>("iconElement");

        int buyAmount = 1;

        void Refresh()
        {
            ownedAmountLabel.text = $"{shopItem.CurrentAmount}";
            buyAmountLabel.text = $"{buyAmount}";
            totalPriceLabel.text = $"{shopItem.Price * buyAmount}";

            buyButton.SetEnabled(CurrencyManager.Instance.HasEnoughMoney(shopItem.Price * buyAmount));
        }

        nameLabel.text = shopItem.Name;
        priceLabel.text = $"{shopItem.Price}";
        iconElement.style.backgroundImage = shopItem.Icon ? new StyleBackground(shopItem.Icon) : null;
        Refresh();

        entry.Q<Button>("plusOne").clicked += () =>
        {
            AudioManager.Instance.Play(clickSound);
            buyAmount = Mathf.Min(99, buyAmount + 1);
            Refresh();
        };
        entry.Q<Button>("minusOne").clicked += () =>
        {
            AudioManager.Instance.Play(clickSound);
            buyAmount = Mathf.Max(1, buyAmount - 1);
            Refresh();
        };
        entry.Q<Button>("plusFive").clicked += () =>
        {
            AudioManager.Instance.Play(clickSound);
            int toAdd = buyAmount == 1 ? 4 : 5;
            buyAmount = Mathf.Min(99, buyAmount + toAdd);
            Refresh();
        };
        entry.Q<Button>("minusFive").clicked += () =>
        {
            AudioManager.Instance.Play(clickSound);
            buyAmount = Mathf.Max(1, buyAmount - 5);
            Refresh();
        };
        buyButton.clicked += () =>
        {
            AudioManager.Instance.Play(buySound);
            shopItem.Buy(buyAmount);

            if (GameState.inTutorial && TutorialManager.Instance != null)
            {
                TutorialManager.Instance.OnItemBought(buyAmount);
            }

            buyAmount = 1;
            Refresh();
        };

        foreach (var btn in entry.Query<Button>().ToList())
        {
            btn.RegisterCallback<MouseEnterEvent>(_ => AudioManager.Instance.Play(hoverSound));
        }
        
        itemContainer.Add(entry);
    }

    private void BuildCategoryTabs()
    {
        tabsContainer.Clear();

        foreach (ShoppingCategory category in System.Enum.GetValues(typeof(ShoppingCategory)))
        {
            if (category == ShoppingCategory.None || category == ShoppingCategory.Items)
                continue;

            var tabTemplate = shopCategoryTabTemplate.Instantiate();
            var tabButton = tabTemplate.Q<Button>("itemsTab");
            var tabText = tabTemplate.Q<Label>("tabText");
            tabButton.clicked += () => ShowCategory(category);
            tabText.text = category.ToString();
            
            tabButton.clicked += () => AudioManager.Instance.Play(clickSound);
            tabButton.RegisterCallback<MouseEnterEvent>(_ => AudioManager.Instance.Play(hoverSound));

            categoryButtons[category] = tabButton;
            tabsContainer.Add(tabTemplate);
        }

        UpdateButtons();
    }

    public void UpdateButtons()
    {
        if (!GameState.inTutorial)
        {
            exitButton.SetEnabled(true);

            foreach (var btn in categoryButtons.Values)
                btn.SetEnabled(true);

            return;
        }

        exitButton.SetEnabled(TutorialManager.Instance != null &&
            TutorialManager.Instance.currentStep >= TutorialStep.ExitShop);

        foreach (var kvp in categoryButtons)
        {
            bool allowed = kvp.Key == ShoppingCategory.Resources;
            kvp.Value.SetEnabled(allowed);
        }
    }
}
