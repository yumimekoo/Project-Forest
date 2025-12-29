using UnityEngine;

public class ShopItem_Furniture : IShopItem
{
    FurnitureSO furniture;

    public ShopItem_Furniture(FurnitureSO furnitureData)
    {
        furniture = furnitureData;
    }
    public int CurrentAmount => FurnitureInventory.Instance.GetAmount(furniture.numericID);
    public string Name => furniture.furnitureName;
    public int Price => furniture.price;
    public ShoppingCategory Category => furniture.shoppingCategory;

    public void Buy(int quantity = 1)
    {
        if (!CurrencyManager.Instance.SpendMoney(Price * quantity))
            return;
        FurnitureInventory.Instance.Add(furniture.numericID, quantity);
    }
}
