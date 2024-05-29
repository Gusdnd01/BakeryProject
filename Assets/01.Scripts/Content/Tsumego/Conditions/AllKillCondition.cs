using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Tsumego/AllKill")]
public class AllKillCondition : TsumegoCondition
{
    public override bool CheckCondition()
    {
        Enemy[] eArr = FindObjectsOfType<Enemy>();
        foreach (Enemy enemy in eArr)
        {
            if(!enemy.HealthCompo.IsDead)
            {
                return false;
            }
        }
        return true;
    }
}
