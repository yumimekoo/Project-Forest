using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class FridgeUI : MonoBehaviour
{
    public static FridgeUI Instance;

    [SerializeField] private UIDocument fridgeUIDoc;
    [SerializeField] private VisualTreeAsset buttonTemp;
    [SerializeField] private SoundSO menuOpen;
    [SerializeField] private SoundSO menuClose;
    [SerializeField] private SoundSO hoverSound;

    private VisualElement 
        root,
        focusCatcher;
    private Label titleLabel;

    Action<ItemDataSO> onItemClicked;

    private void Awake()
    {
        Instance = this;
        root = fridgeUIDoc.rootVisualElement;
        focusCatcher = root.Q<VisualElement>("focusCatcher");
        titleLabel = root.Q<Label>("storageName");
        HideUI();
    }

    private void OnEnable()
    {
        if(!UIManager.Instance) Debug.LogError("UIManager not found!");
        UIManager.Instance.OnEscapePressed += EscapePressed;
    }
    
    private void OnDisable()
    {
        if(!UIManager.Instance) Debug.LogError("UIManager not found!");
        UIManager.Instance.OnEscapePressed -= EscapePressed;
    }

    public void OpenFridge(
        List<ItemDataSO> items, 
        string storageName,
        Func<ItemDataSO, bool> canSelectItem,
        Func<ItemDataSO, int> getAmount,
        Action<ItemDataSO> clickCallback)
    {
        AudioManager.Instance.Play(menuOpen);
        Debug.Log("fridge opened");
        GameTime.SetPaused(true);
        GameState.isInMenu = true;
        GameState.isInStorage = true;
        titleLabel.text = storageName;
        onItemClicked = clickCallback;

        var itemContainer = root.Q<VisualElement>("itemContainer");
        itemContainer.Clear();

        foreach (var item in items)
        {
            int amount = getAmount(item);
            var itemTemplate = buttonTemp.Instantiate();
            var button = itemTemplate.Q<Button>("itemButton");
            var itemIcon = itemTemplate.Q<VisualElement>("itemIcon");
            itemTemplate.Q<Label>("nameLabel").text = $"{item.itemName}";
            itemTemplate.Q<Label>("quantityLabel").text = $"x {amount}";
            
            itemIcon.style.backgroundImage = item.icon ? new StyleBackground(item.icon) : null;
            
            button.clicked += () => OnItemSelected(item);
            itemContainer.Add(itemTemplate);
            button.SetEnabled(canSelectItem(item));
            
            button.RegisterCallback<MouseEnterEvent>(_ =>
            {
                if(canSelectItem(item))
                    AudioManager.Instance.Play(hoverSound);
            });
            
            itemTemplate.style.display = canSelectItem(item) ? DisplayStyle.Flex : DisplayStyle.None;
        }

        focusCatcher.focusable = true;

        ShowUI();
        GameState.playerMovementAllowed = false;
        GameState.playerInteractionAllowed = false;

        focusCatcher.Focus();
    }
    
    private void EscapePressed() {
        if (GameState.isInStorage) CloseUI();
    }

    private void CloseUI()
    {
        AudioManager.Instance.Play(menuClose);
        HideUI();
        Debug.Log("fridge closed");
        GameState.isInMenu = false;
        GameState.isInStorage = false;
        GameTime.SetPaused(false);
        StartCoroutine(EnableInteractionNextFrame());
    }

    private IEnumerator EnableInteractionNextFrame()
    {
        GameState.playerMovementAllowed = false;
        GameState.playerInteractionAllowed = false;

        yield return null;

        GameState.playerMovementAllowed = true;
        GameState.playerInteractionAllowed = true;
    }

    private void OnItemSelected(ItemDataSO selectedItem)
    {
        onItemClicked?.Invoke(selectedItem);
        CloseUI();
    }

    private void HideUI() => root.style.display = DisplayStyle.None;
    private void ShowUI() => root.style.display = DisplayStyle.Flex;
}
