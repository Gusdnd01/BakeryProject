using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CandyParty : Enemy
{
    protected override void Awake()
    {
        base.Awake();
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
        yield return new WaitForSeconds(1.5f);
        for (int i = 0; i < 5; ++i)
        {
            //VFXPlayer.PlayHitEffect(attackParticle, target.transform.position);
            target.HealthCompo.ApplyDamage(CharStat.GetDamage(), this);
            yield return new WaitForSeconds(0.3f);
        }

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
