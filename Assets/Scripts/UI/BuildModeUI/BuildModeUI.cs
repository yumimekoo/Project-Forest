using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class ItemUIRefs
{
    public VisualElement root;
    public Label quantityLabel;
}
public class BuildModeUI : MonoBehaviour
{
    public static BuildModeUI Instance { get; private set; }
    public CameraFollow cameraFollow;
    public BuildMode3D buildMode3D;
    public UIDocument buildModeUI;
    public VisualTreeAsset itemTemplate;

    private Dictionary<int, ItemUIRefs> itemUI = new();
    private VisualElement root;
    private VisualElement
        itemContainer,
        uiHideLeft;
    private Button 
        exitBuildMode,
        btnFurniture,
        btnDecor,
        btnUtility,
        btnAll;

    private void OnEnable()
    {
        if (FurnitureInventory.Instance != null)
            FurnitureInventory.Instance.OnInventoryChanged += UpdateItemQuantity;
    }

    private void OnDisable()
    {
        if (FurnitureInventory.Instance != null)
            FurnitureInventory.Instance.OnInventoryChanged -= UpdateItemQuantity;
    }
    private void Awake()
    {
        // Safe Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        root = buildModeUI.rootVisualElement;
        itemContainer = root.Q<VisualElement>("itemContainer");
        exitBuildMode = root.Q<Button>("exitButton");
        btnDecor = root.Q<Button>("btnDecor");
        btnFurniture = root.Q<Button>("btnFurniture");
        btnUtility = root.Q<Button>("btnUtility");
        btnAll = root.Q<Button>("btnAll");
        uiHideLeft = root.Q<VisualElement>("uiHideLeft");
        btnAll.clicked += () => ShowCategoryAll();
        btnDecor.clicked += () => ShowCategory(BuildCategory.Decorations);
        btnFurniture.clicked += () => ShowCategory(BuildCategory.Furniture);
        btnUtility.clicked += () => ShowCategory(BuildCategory.Utility);
        exitBuildMode.clicked += () => ExitBuildMode();

        itemContainer.RegisterCallback<PointerEnterEvent>(_ => SetPointerOverUI(true));
        itemContainer.RegisterCallback<PointerLeaveEvent>(_ => SetPointerOverUI(false));
        uiHideLeft.RegisterCallback<PointerEnterEvent>(_ => SetPointerOverUI(true));
        uiHideLeft.RegisterCallback<PointerLeaveEvent>(_ => SetPointerOverUI(false));
        UpdateButtons();
        HideUI();
    }

    public void SetPointerOverUI(bool isOverUI)
    {
        buildMode3D.isPointerOverUI = isOverUI;
    }

    public void ShowCategory(BuildCategory category)
    {
        itemContainer.Clear();
        foreach (var item in FurnitureDatabase.Instance.items)
        {
            if (item.buildCategory != category) continue;

            var itemElement = itemTemplate.Instantiate();
            var button = itemElement.Q<Button>("itemButton");
            button.text = item.furnitureName;
            button.clicked += () => buildMode3D.StartBuild(item);
            var quantityLabel = itemElement.Q<Label>("quantityLabel");
            int amount = FurnitureInventory.Instance.GetAmount(item.numericID);
            quantityLabel.text = amount.ToString();
            itemUI[item.numericID] = new ItemUIRefs
            {
                root = itemElement,
                quantityLabel = quantityLabel
            };
            UpdateItemVisualState(item.numericID, amount);
            itemContainer.Add(itemElement);
        }
    }
    // refactor this aswell man 
    public void ShowCategoryAll()
    {
        itemContainer.Clear();
        foreach (var item in FurnitureDatabase.Instance.items)
        {
            var itemElement = itemTemplate.Instantiate();
            var button = itemElement.Q<Button>("itemButton");
            button.text = item.furnitureName;
            button.clicked += () => buildMode3D.StartBuild(item);
            var quantityLabel = itemElement.Q<Label>("quantityLabel");
            int amount = FurnitureInventory.Instance.GetAmount(item.numericID);
            quantityLabel.text = amount.ToString();
            itemUI[item.numericID] = new ItemUIRefs
            {
                root = itemElement,
                quantityLabel = quantityLabel
            };
            UpdateItemVisualState(item.numericID, amount);
            itemContainer.Add(itemElement);
        }
    }

    private void UpdateItemQuantity(int id, int newQuantity)
    {
        if (itemUI.TryGetValue(id, out var ui))
        {
            ui.quantityLabel.text = newQuantity.ToString();
            UpdateItemVisualState(id, newQuantity);
        }
    }

    private void UpdateItemVisualState(int id, int amount)
    {
        if (itemUI.TryGetValue(id, out var uiRefs))
        {
            uiRefs.root.SetEnabled(amount > 0);
        }
    }

    public void ExitBuildMode()
    {
        GameState.playerInteractionAllowed = true;
        GameState.isInBuildMode = false;
        buildMode3D.StopBuild();
        cameraFollow.ChangeFollowTarget(GameState.isInBuildMode);
        HideUI();

        if(GameState.inTutorial && TutorialManager.Instance != null)
        {
             TutorialManager.Instance.OnExitPressed();
        }

    }
    // refactor this shit bro9
    public void UpdateButtons()
    {
        if (!GameState.inTutorial)
        {
            btnDecor.SetEnabled(true);
            btnFurniture.SetEnabled(true);
            btnUtility.SetEnabled(true);
            btnAll.SetEnabled(true);
            exitBuildMode.SetEnabled(true);
            return;
        }

        btnDecor.SetEnabled(false);
        btnFurniture.SetEnabled(false);
        btnUtility.SetEnabled(false);
        btnAll.SetEnabled(TutorialManager.Instance != null &&
                       TutorialManager.Instance.currentStep >= TutorialStep.PlaceAllObjects);
        exitBuildMode.SetEnabled(TutorialManager.Instance != null &&
                       TutorialManager.Instance.currentStep >= TutorialStep.ExitBuildMode);
    }

    public void ShowUI() => buildModeUI.rootVisualElement.style.display = DisplayStyle.Flex;
    public void HideUI() => buildModeUI.rootVisualElement.style.display = DisplayStyle.None;

}
