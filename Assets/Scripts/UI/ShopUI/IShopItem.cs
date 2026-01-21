using UnityEngine;

public interface IShopItem
{
    string Name { get; }
    int Price { get; }
    ShoppingCategory Category { get; }
    int CurrentAmount { get; }

    Sprite Icon { get; }
    void Buy(int quantity = 1);
}
