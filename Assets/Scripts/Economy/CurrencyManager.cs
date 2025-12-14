using System;
using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance;
    public int CurrentMoney { get; private set; }
    public event Action<int> OnMoneyChanged;

    public void RegisterNPC(BasicNPCTest npc)
    {
        npc.OnCorrectOrderGiven += HandleCorrectOrder;
        npc.OnWrongOrderGiven += HandleWrongOrder;
    }

    public void UnregisterNPC(BasicNPCTest npc)
    {
        npc.OnCorrectOrderGiven -= HandleCorrectOrder;
        npc.OnWrongOrderGiven -= HandleWrongOrder;
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void HandleCorrectOrder(int reward)
    {
        AddMoney(reward);
        // Debug.Log($"Earned {reward} money for correct order! Current Money: {CurrentMoney}");
    }
    private void HandleWrongOrder(int penalty)
    {
        int finalPenalty = Mathf.RoundToInt(penalty / 2);
        LoseMoney(finalPenalty);
        // Debug.Log($"Lost {finalPenalty} money for wrong order! Current Money: {CurrentMoney}");
    }
    public void AddMoney(int amount)
    {
        CurrentMoney += amount;
        OnMoneyChanged?.Invoke(CurrentMoney);
    }

    public void LoseMoney(int amount)
    {
        CurrentMoney -= amount;
        CurrentMoney = Mathf.Max(CurrentMoney, 0);
        OnMoneyChanged?.Invoke(CurrentMoney);
    }

    public bool SpendMoney(int amount)
    {
        if(CurrentMoney < amount)
            {
            // Debug.Log("Not enough money!");
            return false;
        }

        CurrentMoney -= amount;
        OnMoneyChanged?.Invoke(CurrentMoney);
        return true;
    }

    public void SetMoney(int amount)
    {
        CurrentMoney = amount;
        OnMoneyChanged?.Invoke(CurrentMoney);
    }

}
