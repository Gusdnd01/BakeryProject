using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleEnemy : Enemy
{
    protected override void Awake()
    {
        base.Awake();
        OnAnimationCall += () => target.HealthCompo.ApplyDamage(CharStat.GetDamage(), this);
    }

    protected override void Start()
    {
        base.Start();
        target = BattleController?.Player;
    }


    public override void Attack()
    {
        OnAttackStart?.Invoke();
        AnimatorCompo.SetBool(attackAnimationHash, true);
        MoveToTargetForward(Vector3.zero);
        OnAnimationEnd += () =>
        {
            MoveToOriginPos();
            AnimatorCompo.SetBool(attackAnimationHash, false);
            //CameraController.Instance.SetDefaultCam();
            OnAnimationEnd = null;
        };
        OnAttackEnd?.Invoke();
    }

    public override void SlowEntityBy(float percent)
    {
    }

    public override void TurnAction()
    {
        Attack();
    }

    public override void TurnEnd()
    {

    }
    public override void TurnStart()
    {
        turnStatus = TurnStatus.Ready;
    }

    protected override void HandleEndMoveToTarget()
    {
        MoveToOriginPos();
        target.HealthCompo.ApplyDamage(CharStat.GetDamage(), this);
        AnimatorCompo.SetTrigger(attackTriggerAnimationHash);
    }

    protected override void HandleEndMoveToOriginPos()
    {
        turnStatus = TurnStatus.End;
    }
}
