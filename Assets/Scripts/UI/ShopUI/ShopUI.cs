using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class ShopUI : MonoBehaviour
{
    public static ShopUI Instance;
    public UIDocument shopUI;
    public VisualTreeAsset shopItemTemplate;

    private VisualElement
        itemContainer,
        tabsContainer;
    private Button
        exitButton;
    private ShoppingCategory currentCategory;
    List<IShopItem> allShopItems;

    private Dictionary<ShoppingCategory, Button> categoryButtons = new();

    private void Awake()
    {
        Instance = this;
        InitUI();
    }

    private void Start()
    {
        UpdateButtons();
    }

    private void InitUI()
    {
        var root = shopUI.rootVisualElement;
        itemContainer = root.Q<VisualElement>("itemContainer");
        tabsContainer = root.Q<VisualElement>("tabsContainer");
        exitButton = root.Q<Button>("exitButton");

        exitButton.clicked += OnExitButton;

        BuildShopItems();
        BuildCategoryTabs();

        ShowCategory(ShoppingCategory.Items);
        HideUI();
    }

    public void ShowUI()
    {
        BuildShopItems();
        ShowCategory(currentCategory);
        shopUI.rootVisualElement.style.display = DisplayStyle.Flex;
        GameState.playerInteractionAllowed = false;
        GameState.playerMovementAllowed = false;
    }

    public void HideUI()
    {
        shopUI.rootVisualElement.style.display = DisplayStyle.None;
        GameState.playerInteractionAllowed = true;
        GameState.playerMovementAllowed = true;
    }

    private void OnExitButton()
    {
        HideUI();

        if (GameState.inTutorial && TutorialManager.Instance != null)
        {
            TutorialManager.Instance.OnExitPressed();
        }
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
        Refresh();

        entry.Q<Button>("plusOne").clicked += () =>
        {
            buyAmount = Mathf.Min(99, buyAmount + 1);
            Refresh();
        };
        entry.Q<Button>("minusOne").clicked += () =>
        {
            buyAmount = Mathf.Max(1, buyAmount - 1);
            Refresh();
        };
        entry.Q<Button>("plusFive").clicked += () =>
        {
            buyAmount = Mathf.Min(99, buyAmount + 5);
            Refresh();
        };
        entry.Q<Button>("minusFive").clicked += () =>
        {
            buyAmount = Mathf.Max(1, buyAmount - 5);
            Refresh();
        };
        buyButton.clicked += () =>
        {
            shopItem.Buy(buyAmount);

            if (GameState.inTutorial && TutorialManager.Instance != null)
            {
                TutorialManager.Instance.OnItemBought(buyAmount);
            }

            Refresh();
        };
        itemContainer.Add(entry);
    }

    private void BuildCategoryTabs()
    {
        tabsContainer.Clear();

        foreach (ShoppingCategory category in System.Enum.GetValues(typeof(ShoppingCategory)))
        {
            if (category == ShoppingCategory.None)
                continue;

            var tabButton = new Button(() => ShowCategory(category))
            {
                text = category.ToString()
            };

            categoryButtons[category] = tabButton;
            tabsContainer.Add(tabButton);
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
            bool allowed = kvp.Key == ShoppingCategory.Items;
            kvp.Value.SetEnabled(allowed);
        }
    }
}
