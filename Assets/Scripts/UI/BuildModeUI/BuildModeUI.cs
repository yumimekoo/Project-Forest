using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BuildModeUI : MonoBehaviour
{
    public static BuildModeUI Instance { get; private set; }
    public CameraFollow cameraFollow;
    public BuildMode3D buildMode3D;
    public UIDocument buildModeUI;
    public VisualTreeAsset itemTemplate;

    private Dictionary<int, Label> quantityLabels = new();
    private VisualElement root;
    private VisualElement
        itemContainer,
        uiHideLeft;
    private Button 
        exitBuildMode,
        btnFurniture,
        btnDecor,
        btnUtility;

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
        uiHideLeft = root.Q<VisualElement>("uiHideLeft");
        btnDecor.clicked += () => ShowCategory(BuildCategory.Decorations);
        btnFurniture.clicked += () => ShowCategory(BuildCategory.Furniture);
        btnUtility.clicked += () => ShowCategory(BuildCategory.Utility);
        exitBuildMode.clicked += () => ExitBuildMode();

        itemContainer.RegisterCallback<PointerEnterEvent>(_ => SetPointerOverUI(true));
        itemContainer.RegisterCallback<PointerLeaveEvent>(_ => SetPointerOverUI(false));
        uiHideLeft.RegisterCallback<PointerEnterEvent>(_ => SetPointerOverUI(true));
        uiHideLeft.RegisterCallback<PointerLeaveEvent>(_ => SetPointerOverUI(false));

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
            quantityLabel.text = FurnitureInventory.Instance.GetAmount(item.numericID).ToString();
            quantityLabels[item.numericID] = quantityLabel;
            itemContainer.Add(itemElement);
        }
    }

    private void UpdateItemQuantity(int id, int newQuantity)
    {
        if (quantityLabels.TryGetValue(id, out var label))
        {
            label.text = newQuantity.ToString();
        }
    }

    public void ExitBuildMode()
    {
        GameState.playerInteractionAllowed = true;
        GameState.isInBuildMode = false;
        buildMode3D.StopBuild();
        cameraFollow.ChangeFollowTarget(GameState.isInBuildMode);
        HideUI();
    }

    public void ShowUI() => buildModeUI.rootVisualElement.style.display = DisplayStyle.Flex;
    public void HideUI() => buildModeUI.rootVisualElement.style.display = DisplayStyle.None;

}
