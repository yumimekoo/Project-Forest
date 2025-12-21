using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DrinkOrder
{
    public bool isSpecific;
    public ItemDataSO requestedDrink;

    public override string ToString()
    {
        if(isSpecific && requestedDrink != null)
        {
            return $"Order: {requestedDrink.itemName}";
        }
        else
        {
            return "Surprise me! Order";
        }
    }
}

public static class ListExtensions
{
    public static T RandomItem<T>(this List<T> list)
    {
        if (list == null || list.Count == 0)
            return default;
        return list[Random.Range(0, list.Count)];
    }
}

public static class NPCOrderGenerator
{
    public static DrinkOrder GenerateOrder(NPCIdentitySO npc)
    {
        
        List<ItemDataSO> availableDrinks = UnlockManager.Instance.runtimeDatabase.GetUnlockedItems()
            .Where(i => i.itemType == ItemType.Drink).ToList();

        if(availableDrinks.Count == 0)
        {
            Debug.LogWarning("No available drinks to order.");
            return null;
        }

        bool wantsSpecificDrink = Random.value < 0.85f; // % chance to want a specific drink, maybe later adjust based on NPC level freindship

        if (wantsSpecificDrink)
        {
            var favAvailable = npc.favDrinks
                .Where(d => availableDrinks.Contains(d)).ToList();

            if(favAvailable.Count > 0)
            {
                return new DrinkOrder
                {
                    isSpecific = true,
                    requestedDrink = favAvailable.RandomItem()
                };
            }

            return new DrinkOrder
            {
                isSpecific = true,
                requestedDrink = availableDrinks.RandomItem()
            };

        }

        return new DrinkOrder
        {
            isSpecific = false,
            requestedDrink = null
        };
    }
}
