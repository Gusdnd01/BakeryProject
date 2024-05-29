using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : BossBrain
{
    [SerializeField] private BuffSO pattenBuff;
    protected override void OnEnable()
    {
        base.OnEnable();
        BuffStatCompo.AddBuff(pattenBuff,5);
    }
    protected override void OnDisable()
    {

        base.OnDisable();
    }
    public override void SlowEntityBy(float percent)
    {

    }

    protected override void HandleEndMoveToOriginPos()
    {

    }

    protected override void HandleEndMoveToTarget()
    {

    }
}
