using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PumpkinBat : Enemy
{
    protected override void Start()
    {
        base.Start();
        VFXPlayer.OnEndEffect += () => turnStatus = TurnStatus.End;
    }

    public override void Attack()
    {
        OnAttackStart?.Invoke();
        VFXPlayer.PlayParticle(attackParticle);

        StartCoroutine(AttackCor());
    }

    private IEnumerator AttackCor()
    {
        for (int i = 0; i < 3; i++)
        {
            yield return new WaitForSeconds(0.15f);
            target.HealthCompo.ApplyDamage(CharStat.GetDamage(), this);
            //VFXPlayer.PlayHitEffect(attackParticle, target.transform.position);
        }
        yield return new WaitForSeconds(1f);
        turnStatus = TurnStatus.End;
        OnAttackEnd?.Invoke();
    }

    public override void SlowEntityBy(float percent)
    {
    }

    public override void TurnAction()
    {
        turnStatus = TurnStatus.Running;
        Attack();
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
    }

    protected override void HandleEndMoveToTarget()
    {
    }
}

