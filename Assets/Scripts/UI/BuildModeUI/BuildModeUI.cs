using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class ItemUIRefs
{
    public VisualElement root;
    public Button button;
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
    private int? selectedItemId = null;
    private VisualElement root;
    private VisualElement
        rootBackground,
        itemContainer,
        uiHideLeft;
    private Button 
        exitBuildMode,
        btnFurniture,
        btnDecor,
        btnUtility,
        btnAll,
        deleteButton;

    private void OnEnable()
    {
        if (FurnitureInventory.Instance != null)
            FurnitureInventory.Instance.OnInventoryChanged += UpdateItemQuantity;
        buildMode3D.deleteModeChanged += CheckDeletionMode;
    }

    private void OnDisable()
    {
        if (FurnitureInventory.Instance != null)
            FurnitureInventory.Instance.OnInventoryChanged -= UpdateItemQuantity;
        buildMode3D.deleteModeChanged -= CheckDeletionMode;
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
        rootBackground = root.Q<VisualElement>("rootBackground");
        itemContainer = root.Q<VisualElement>("itemContainer");
        exitBuildMode = root.Q<Button>("exitButton");
        btnDecor = root.Q<Button>("btnDecor");
        btnFurniture = root.Q<Button>("btnFurniture");
        btnUtility = root.Q<Button>("btnUtility");
        btnAll = root.Q<Button>("btnAll");
        deleteButton = root.Q<Button>("deleteButton");
        uiHideLeft = root.Q<VisualElement>("uiHideLeft");
        btnAll.clicked += () => ShowCategoryAll();
        btnDecor.clicked += () => ShowCategory(BuildCategory.Decorations);
        btnFurniture.clicked += () => ShowCategory(BuildCategory.Furniture);
        btnUtility.clicked += () => ShowCategory(BuildCategory.Utility);
        exitBuildMode.clicked += () => ExitBuildMode();
        deleteButton.clicked += () => buildMode3D.HandleDeleteToggle();

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

    private void BuildItems(IEnumerable<FurnitureSO> items)
    {
        itemContainer.Clear();
        itemUI.Clear();

        foreach (var item in items)
        {
            var itemElement = itemTemplate.Instantiate();
            var button = itemElement.Q<Button>("itemButton");
            button.text = item.furnitureName;

            int id = item.numericID;

            button.clicked += () =>
            {
                SelectItem(id);
                buildMode3D.StartBuild(item);
            };

            var quantityLabel = itemElement.Q<Label>("quantityLabel");
            int amount = FurnitureInventory.Instance.GetAmount(id);
            quantityLabel.text = amount.ToString();
            itemUI[id] = new ItemUIRefs
            {
                root = itemElement,
                button = button,
                quantityLabel = quantityLabel
            };

            UpdateItemVisualState(id, amount);
            itemContainer.Add(itemElement);
        }
    }

    private void SelectItem(int id)
    {
        if(selectedItemId.HasValue && itemUI.TryGetValue(selectedItemId.Value, out var oldUI))
        {
            oldUI.button.RemoveFromClassList("selected");
        }
        if(itemUI.TryGetValue(id, out var newUI))
        {
            newUI.button.AddToClassList("selected");
            selectedItemId = id;
        }
    }

    public void ShowCategory(BuildCategory category)
    {
        CheckSelectedCategory(category);
        BuildItems(FurnitureDatabase.Instance.items.Where(item => item.buildCategory == category));
    }

    public void CheckDeletionMode(bool isInDeleteMode)
    {
        if (isInDeleteMode)
        {
            rootBackground.AddToClassList("selected");
            uiHideLeft.style.display = DisplayStyle.None;
            itemContainer.style.display = DisplayStyle.None;
        }
        else
        {
            rootBackground.RemoveFromClassList("selected");
            uiHideLeft.style.display = DisplayStyle.Flex;
            itemContainer.style.display = DisplayStyle.Flex;
        }
    }
    // refactor this aswell man 
    public void ShowCategoryAll()
    {
        CheckSelectedCategory(BuildCategory.None);
        BuildItems(FurnitureDatabase.Instance.items);
    }

    public void CheckSelectedCategory(BuildCategory category)
    {
        btnDecor.RemoveFromClassList("selected");
        btnFurniture.RemoveFromClassList("selected");
        btnUtility.RemoveFromClassList("selected");
        btnAll.RemoveFromClassList("selected");
        switch (category)
        {
            case BuildCategory.Decorations:
                btnDecor.AddToClassList("selected");
                break;
            case BuildCategory.Furniture:
                btnFurniture.AddToClassList("selected");
                break;
            case BuildCategory.Utility:
                btnUtility.AddToClassList("selected");
                break;
            default:
                btnAll.AddToClassList("selected");
                break;
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
    public void HideUI()
    {
        buildModeUI.rootVisualElement.style.display = DisplayStyle.None;
        rootBackground.RemoveFromClassList("selected");
        uiHideLeft.style.display = DisplayStyle.Flex;
        itemContainer.style.display = DisplayStyle.Flex;
        btnDecor.RemoveFromClassList("selected");
        btnFurniture.RemoveFromClassList("selected");
        btnUtility.RemoveFromClassList("selected");
        btnAll.RemoveFromClassList("selected");
        itemContainer.Clear();
    }

}
