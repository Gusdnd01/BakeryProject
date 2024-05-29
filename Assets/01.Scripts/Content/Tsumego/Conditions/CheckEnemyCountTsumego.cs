using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CheckEnemyCountCondition
{
    Upper,
    Lower,
    Same
}

[CreateAssetMenu(menuName = "SO/Tsumego/CheckEnemyCount")]
public class CheckEnemyCountTsumego : TsumegoCondition
{
    public int Limit;
    public CheckEnemyCountCondition LimitCondition;
    BattleController _battleController;

    public override bool CheckCondition()
    {
        if(_battleController == null)
        {
            _battleController = FindObjectOfType<BattleController>();
        }

        switch (LimitCondition)
        {
            case CheckEnemyCountCondition.Upper:
                return _battleController.DeathEnemyList.Count > Limit;
            case CheckEnemyCountCondition.Lower:
                return _battleController.DeathEnemyList.Count < Limit;
            case CheckEnemyCountCondition.Same:
                return _battleController.DeathEnemyList.Count == Limit;
            default:
                return true;
        }
    }
}
