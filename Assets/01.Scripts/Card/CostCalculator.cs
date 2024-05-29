using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class CostCalculator
{
    public static int CurrentMoney { get; set; } = 10;
    public static int CurrentExMana { get; private set; }
    public static Action<int> MoneyChangeEvent;
    public static Action<int> ExtraManaChangeEvent;

    public static void Init()
    {
        CurrentMoney = 10;
        CurrentExMana = 0;

        MoneyChangeEvent?.Invoke(CurrentMoney);
        ExtraManaChangeEvent?.Invoke(CurrentExMana);
    }

    public static void GetCost(int toGetCost)
    {
        CurrentMoney += toGetCost;
        MoneyChangeEvent?.Invoke(CurrentMoney);
    }

    public static void UseCost(int toUseCost, bool isSkillCard)
    {
        if(isSkillCard)
        {
            CurrentMoney = Mathf.Clamp(CurrentMoney - toUseCost, 0, int.MaxValue);
            MoneyChangeEvent?.Invoke(CurrentMoney);
        }
        else
        {
            if(toUseCost <= CurrentExMana)
            {
                CurrentExMana -= toUseCost;
                ExtraManaChangeEvent?.Invoke(CurrentExMana);
            }
            else
            {
                int remain = toUseCost - CurrentExMana;
                CurrentExMana = 0;

                CurrentMoney -= remain;
                ExtraManaChangeEvent?.Invoke(CurrentExMana);
                MoneyChangeEvent?.Invoke(CurrentMoney);
            }
        }
    }

    public static void GetExMana(int togetMana)
    {
        CurrentExMana = Math.Clamp(CurrentExMana + togetMana, 0, 3);
        ExtraManaChangeEvent?.Invoke(CurrentExMana);
    }

    private static void UseExMana(int toUseMana)
    {
        CurrentExMana = Mathf.Clamp(CurrentExMana - toUseMana, 0, 3);
        ExtraManaChangeEvent?.Invoke(CurrentExMana);
    }

    public static bool CanUseCost(int toUseCost, bool isSkillCard)
    {
        if (isSkillCard)
            return toUseCost <= CurrentMoney;
        else
            return toUseCost <= CurrentMoney + CurrentExMana;
    }
}
