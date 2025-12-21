using System.Collections.Generic;
using UnityEngine;

public enum OrderOutcome
{
    Perfect,
    Correct,
    Wrong
}

public class OrderResult
{
    public OrderOutcome outcome;
    public int moneyDelta;
    public int friendshipDelta;
}

public static class NPCOrderResolver
{
    public static OrderResult Evaluate(
        NPCIdentitySO npc,
        DrinkOrder order,
        ItemDataSO givenDrink,
        List<ItemDataSO> contents)
    {
        if (order == null || givenDrink == null)
            return null;

        bool correctDrink = order.isSpecific && order.requestedDrink == givenDrink;

        int money = 0;
        int friendship = 0;

        if (order.isSpecific)
        {
            if (correctDrink)
            {
                bool isFavorite = npc.favDrinks.Contains(givenDrink);

                if (isFavorite)
                {
                    money = 20; // later in the npc thingy? 
                    friendship = 5;
                    return Result(OrderOutcome.Perfect, money, friendship);
                }
                else
                {
                    money = 15;
                    friendship = 2;
                    return Result(OrderOutcome.Correct, money, friendship);
                }
            }
            else
            {
                money = 3;
                friendship = -3;
                return Result(OrderOutcome.Wrong, money, friendship);
            }
        }

        int ingredientMatches = ScoreIngredients(npc, contents);

        if (ingredientMatches > 0)
        {
            money = 10 + (ingredientMatches * 2);
            friendship = 1 + ingredientMatches;
            return Result(OrderOutcome.Correct, money, friendship);
        }

        else if (ingredientMatches < 0)
        {
            money = 2;
            friendship = -1 * (ingredientMatches);
            return Result(OrderOutcome.Wrong, money, friendship);
        }

        money = 5;
        friendship = 0;
        return Result(OrderOutcome.Correct, money, friendship);
    }

    private static int ScoreIngredients(NPCIdentitySO npc, List<ItemDataSO> contents)
    {
        int matches = 0;
        foreach (var ingredient in contents)
        {
            if (npc.likedIngredients.Contains(ingredient))
            {
                matches++;
            }
            if (npc.dislikedIngredients.Contains(ingredient))
            {
                matches--;
            }
        }
        return matches;
    }

    private static OrderResult Result(OrderOutcome outcome, int money, int friendship)
    {
        return new OrderResult
        {
            outcome = outcome,
            moneyDelta = money,
            friendshipDelta = friendship
        };
    }
}
