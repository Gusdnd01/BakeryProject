using BTVisual;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttack : SampleBossNode, IAnimationEventHandler, IAnimationEndHandler
{
    private bool isAttacking;
    public void OnAnimationEndHandle()
    {
        isAttacking = false;
    }

    public void OnAnimationEventHandle()
    {
        brain.target.HealthCompo.ApplyDamage(brain.CharStat.GetDamage() * 2, brain);
        FeedbackManager.Instance.ShakeScreen(4f);
    }

    protected override void OnStart()
    {
        brain.OnMoveTarget += base.OnStart;
        brain.MoveToTargetForward(brain.target.forwardTrm.position);

        brain.BossAnimator.OnAnimationEvent += OnAnimationEventHandle;
        brain.BossAnimator.OnAnimationEnd += OnAnimationEndHandle;
        isAttacking = true;
    }

    protected override void OnStop()
    {
        brain.OnMoveOriginPos += base.OnStop;
        brain.MoveToOriginPos();

        brain.BossAnimator.OnAnimationEvent -= OnAnimationEventHandle;
        brain.BossAnimator.OnAnimationEnd -= OnAnimationEndHandle;
    }

    protected override State OnUpdate()
    {
        if (isAttacking)
        {
            return State.RUNNING;
        }
        return State.SUCCESS;
    }
}
