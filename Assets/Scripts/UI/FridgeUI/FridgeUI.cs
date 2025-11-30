using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class FridgeUI : MonoBehaviour
{
    public static FridgeUI Instance;

    [SerializeField] UIDocument fridgeUIDoc;

    private VisualElement root;

    Action<ItemDataSO> onItemClicked;

    private void Awake()
    {
        Instance = this;
        root = fridgeUIDoc.rootVisualElement;
        HideUI();
    }

    public void OpenFridge(List<ItemDataSO> items, Action<ItemDataSO> clickCallback)
    {
        onItemClicked = clickCallback;
        var itemContainer = root.Q<VisualElement>("ItemContainer");
        itemContainer.Clear();

        foreach (var item in items)
        {
            var button = new Button(() => OnItemSelected(item)) { text = item.itemName };
            itemContainer.Add(button);
        }
        ShowUI();
        GameState.playerMovementAllowed = false;
        GameState.playerInteractionAllowed = false;
    }

    private void OnItemSelected(ItemDataSO selectedItem)
    {
        onItemClicked?.Invoke(selectedItem);
        HideUI();
        GameState.playerMovementAllowed = true;
        GameState.playerInteractionAllowed = true;
    }

    private void HideUI() => root.style.display = DisplayStyle.None;
    private void ShowUI() => root.style.display = DisplayStyle.Flex;
}
