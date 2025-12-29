public interface IShopItem
{
    string Name { get; }
    int Price { get; }
    ShoppingCategory Category { get; }
    int CurrentAmount { get; }

    void Buy(int quantity = 1);
}
