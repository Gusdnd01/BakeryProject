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
                // ����
                return;
            }
        }
        // ���� ���� �����
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
        // SO�� Ŭ���� ó��
        CurTsumegoInfo.IsClear = true;
        _gameEndEvent?.Invoke(true);
        _stageClearEvent?.Invoke(true);
        // Ŭ���� ����, ���� ����, Ŭ���� ������ ���� ó��
    }

    public void DefeatStage()
    {
        CurTsumegoInfo.IsClear = false;
        _gameEndEvent?.Invoke(true);
        _stageClearEvent?.Invoke(false);
    }
}