using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class FridgeUI : MonoBehaviour
{
    public static FridgeUI Instance;

    [SerializeField] UIDocument fridgeUIDoc;
    [SerializeField] VisualTreeAsset buttonTemp;

    private VisualElement 
        root,
        focusCatcher;

    Action<ItemDataSO> onItemClicked;

    private void Awake()
    {
        Instance = this;
        root = fridgeUIDoc.rootVisualElement;
        focusCatcher = root.Q<VisualElement>("focusCatcher");
        HideUI();
    }

    public void OpenFridge(
        List<ItemDataSO> items, 
        Func<ItemDataSO, bool> canSelectItem,
        Func<ItemDataSO, int> getAmount,
        Action<ItemDataSO> clickCallback)
    {

        onItemClicked = clickCallback;

        var itemContainer = root.Q<VisualElement>("ItemContainer");
        itemContainer.Clear();

        foreach (var item in items)
        {
            int amount = getAmount(item);
            var button = new Button(() => OnItemSelected(item)) { text = $"{item.itemName} x{amount}" };
            button.style.height = 20;
            itemContainer.Add(button);
            button.SetEnabled(canSelectItem(item));
        }

        focusCatcher.focusable = true;
        focusCatcher.RegisterCallback<KeyDownEvent>(OnKeyDown);

        ShowUI();
        GameState.playerMovementAllowed = false;
        GameState.playerInteractionAllowed = false;

        focusCatcher.Focus();
    }

    private void OnKeyDown(KeyDownEvent evt)
    {
        //Debug.Log($"Key pressed: {evt.keyCode}");
        if (evt.keyCode == KeyCode.Escape)
        {
            CloseUI();
            evt.StopPropagation();
        }
    }

    private void CloseUI()
    {
        focusCatcher.UnregisterCallback<KeyDownEvent>(OnKeyDown);
        HideUI();

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
