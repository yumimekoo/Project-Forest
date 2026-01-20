using System;
using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance;
    public int CurrentMoney { get; private set; }
    public event Action<int> OnMoneyChanged;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
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

        LoseMoney(amount);
        OnMoneyChanged?.Invoke(CurrentMoney);
        return true;
    }

    public bool HasEnoughMoney(int amount)
    {
        return CurrentMoney >= amount;
    }

    public void SetMoney(int amount)
    {
        CurrentMoney = amount;
        OnMoneyChanged?.Invoke(CurrentMoney);
    }

}
