using UnityEngine;
using UnityEngine.UIElements;

public class BuildModeUI : MonoBehaviour
{
    public static BuildModeUI Instance { get; private set; }
    public CameraFollow cameraFollow;
    public BuildMode3D buildMode3D;
    public UIDocument buildModeUI;
    public VisualTreeAsset itemTemplate;

    private VisualElement root;
    private VisualElement
        itemContainer;
    private Button 
        exitBuildMode,
        btnFurniture,
        btnDecor,
        btnUtility;

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
        btnDecor.clicked += () => ShowCategory(BuildCategory.Decorations);
        btnFurniture.clicked += () => ShowCategory(BuildCategory.Furniture);
        btnUtility.clicked += () => ShowCategory(BuildCategory.Utility);
        exitBuildMode.clicked += () => ExitBuildMode();

        HideUI();
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
            itemContainer.Add(itemElement);
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
