using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AilmentStat
{
    private Health _health;

    private Dictionary<AilmentEnum, int> _ailmentStack;

    public AilmentEnum currentAilment; //질병 및 디버프 상태

    public event Action<AilmentEnum> EndOFAilmentEvent; // 상태이상 종료시 발생

    public AilmentStat(Health health)
    {
        _ailmentStack = new Dictionary<AilmentEnum, int>();

        _health = health;

        foreach (AilmentEnum ailment in Enum.GetValues(typeof(AilmentEnum)))
        {
            if (ailment == AilmentEnum.None) continue;
            _ailmentStack.Add(ailment, 0);
        }
    }

    public void UpdateAilment()
    {
        AilmentDamage();
    }
    public void CuredAilment(AilmentEnum ailment)
    {
        currentAilment ^= ailment; //XOR로 빼주고
        EndOFAilmentEvent?.Invoke(ailment); //종료 알림.
    }

    private void AilmentDamage()
    {
        foreach (AilmentEnum ailment in Enum.GetValues(typeof(AilmentEnum)))
        {
            if (HasAilment(ailment))
            {
                switch (ailment)
                {
                    default:
                        break;
                }
            }
        }
    }
    public void UsedToAilment(AilmentEnum ailment)
    {
        if (!HasAilment(ailment))
            return;


        switch (ailment)
        {
            case AilmentEnum.None:
                break;
            case AilmentEnum.Chilled:
                break;
            case AilmentEnum.Shocked:
                //_ailmentStack[ailment] = 0;
                //CuredAilment(ailment);
                _health.AilmentByDamage(ailment, Mathf.RoundToInt(_health.maxHealth * 0.07f));
                break;
        }
    }

    //특정 디버프가 존재하는지 체크
    public bool HasAilment(AilmentEnum ailment)
    {
        bool temp = ((currentAilment & ailment) > 0);
        return (currentAilment & ailment) > 0;
    }

    public int GetStackAilment(AilmentEnum ailment)
    {
        return _ailmentStack[ailment];
    }

    public void ApplyAilments(AilmentEnum value, int stack = 1)
    {
        Debug.Log(value);
        currentAilment |= value; //현재 상태이상에 추가 상태이상 덧씌우고

        //상태이상 새로 들어온 애들은 시간 갱신해주고. 
        SetAilment(value, stack);

        if ((value & AilmentEnum.Chilled) > 0 && _ailmentStack[value] >= 5)
        {
            _ailmentStack[value] -= 5;
            if (_ailmentStack[value] <= 0)
                CuredAilment(value);
            _health.IsFreeze = true;
        }
    }



    //질병효과와 지속시간 셋팅
    private void SetAilment(AilmentEnum ailment, int stack = 1)
    {
        _ailmentStack[ailment] += stack;
    }
}