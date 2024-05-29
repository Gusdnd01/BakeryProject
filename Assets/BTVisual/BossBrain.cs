using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBrain : Enemy
{
    public BossAnimator BossAnimator { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        BossAnimator = transform.Find("Visual").GetComponent<BossAnimator>();
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

    public override void Attack()
    {
    }

    public override void TurnAction()
    {
        turnStatus = TurnStatus.Running;
    }
}
