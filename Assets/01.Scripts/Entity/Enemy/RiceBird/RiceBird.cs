using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiceBird : Enemy
{
    public override void Attack()
    {
        OnAttackStart?.Invoke();
        StartCoroutine(AttackCor());
    }
    private IEnumerator AttackCor()
    {
        yield return new WaitForSeconds(0.9f);
        //VFXPlayer.PlayHitEffect(attackParticle, target.transform.position);
        target.HealthCompo.ApplyDamage(CharStat.GetDamage(), this);
        OnAttackEnd?.Invoke();
    }
    public override void SlowEntityBy(float percent)
    {
    }
    public override void TurnStart()
    {
        base.TurnStart();
    }
    public override void TurnAction()
    {
        turnStatus = TurnStatus.Running;
        AnimatorCompo.SetTrigger(attackTriggerAnimationHash);
        MoveToTargetForward(Vector3.zero);
    }
    public override void TurnEnd()
    {
        base.TurnEnd();
    }
    public override void MoveToOriginPos()
    {
        Vector3 pos = lastMovePos;
        pos.y += 10;
        transform.position = pos;
        transform.DOMove(lastMovePos, 1f).OnComplete(() =>
        {
            HandleEndMoveToTarget();
            AnimatorCompo.SetBool(attackAnimationHash, false);
        });

    }
    protected override void HandleEndMoveToOriginPos()
    {
        turnStatus = TurnStatus.End;

    }
    public override void MoveToTargetForward(Vector3 pos)
    {


        lastMovePos = transform.position;

        Attack();

        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOMoveY(target.transform.position.y, 0.1f));
        seq.Append(transform.DOMoveX(transform.position.x - 3, 0.4f));
        seq.Append(transform.DOMoveX(target.transform.position.x + 5, 1f));
        seq.Insert(1f, transform.DOMoveY(target.transform.position.y, 0.5f));
        seq.OnComplete(HandleEndMoveToTarget);
    }
    
    protected override void HandleEndMoveToTarget()
    {
        MoveToOriginPos();
    }
}
