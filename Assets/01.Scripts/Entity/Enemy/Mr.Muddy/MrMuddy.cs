using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MrMuddy : Enemy
{
    protected override void Start()
    {
        base.Start();
    }

    public override void Attack()
    {
        target.HealthCompo.ApplyDamage(CharStat.GetDamage(),this);
        //VFXPlayer.PlayHitEffect(attackParticle, target.transform.position);
        MoveToOriginPos();
        OnAttackEnd?.Invoke();
    }

    public override void SlowEntityBy(float percent)
    {
    }

    public override void TurnAction()
    {
        turnStatus = TurnStatus.Running;
        AnimatorCompo.SetBool(attackAnimationHash,true);
        OnAttackStart?.Invoke();
        MoveToTargetForward(Vector3.zero);
    }

    public override void TurnEnd()
    {
        base.TurnEnd();
    }

    public override void TurnStart()
    {
        base.TurnStart();
        
        turnStatus = TurnStatus.Ready;
    }

    protected override void HandleEndMoveToOriginPos()
    {
        turnStatus = TurnStatus.End;
    }

    protected override void HandleEndMoveToTarget()
    {
        Attack();
    }
}
