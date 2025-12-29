using UnityEngine;

public class ShopItem_Item : IShopItem
{
    ItemDataSO item;
    public ShopItem_Item(ItemDataSO itemData)
    {
        item = itemData;
    }
    public int CurrentAmount => ItemsInventory.Instance.GetAmount(item.id);
    public string Name => item.itemName;
    public int Price => item.price;
    public ShoppingCategory Category => item.shoppingCategory;
    public void Buy(int quantity = 1)
    {
        if (!CurrencyManager.Instance.SpendMoney(Price * quantity))
            return;
        
        ItemsInventory.Instance.Add(item.id, quantity);
    }
}
