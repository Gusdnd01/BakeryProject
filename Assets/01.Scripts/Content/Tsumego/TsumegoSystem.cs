using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TsumegoSystem : MonoBehaviour
{
    [SerializeField] private UnityEvent<bool> _stageClearEvent;
    [SerializeField] private UnityEvent<bool> _gameEndEvent; 
    public TsumegoInfo CurTsumegoInfo { get; set; }

    public void CheckClear()
    {
        foreach(var condition in CurTsumegoInfo.Conditions)
        {
            if (!condition.CheckCondition())
            {
                // 실패
                return;
            }
        }
        // 조건 전부 통과함
        ClearStage();
    }

    public void CheckDefeat()
    {
        foreach(var condition in CurTsumegoInfo.DefeatConditions)
        {
            if (!condition.CheckCondition())
            {
                return;
            }
        }
        DefeatStage();
    }

    public void ClearStage()
    {
        // SO에 클리어 처리
        CurTsumegoInfo.IsClear = true;
        _gameEndEvent?.Invoke(true);
        _stageClearEvent?.Invoke(true);
        // 클리어 연출, 보상 지급, 클리어 데이터 갱신 처리
    }

    public void DefeatStage()
    {
        CurTsumegoInfo.IsClear = false;
        _gameEndEvent?.Invoke(true);
        _stageClearEvent?.Invoke(false);
    }
}
