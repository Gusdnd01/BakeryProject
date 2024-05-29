using BTVisual;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunAttack : SampleBossNode, IAnimationEventHandler
{
    private readonly int _fireHash = Animator.StringToHash("Fire");

    [SerializeField] private int _attackCnt;
    private int _remainCnt;

    public void OnAnimationEventHandle()
    {
        brain.target.HealthCompo.ApplyDamage(Mathf.RoundToInt(brain.CharStat.GetDamage() * 0.3f), brain);
        FeedbackManager.Instance.ShakeScreen(1.5f);
        if (--_remainCnt > 0) brain.AnimatorCompo.SetTrigger(_fireHash);
    }

    protected override void OnStart()
    {
        base.OnStart();
        _remainCnt = _attackCnt;
        brain.AnimatorCompo.SetTrigger(_fireHash);

        brain.BossAnimator.OnAnimationEvent += OnAnimationEventHandle;
    }

    protected override void OnStop()
    {
        brain.BossAnimator.OnAnimationEvent -= OnAnimationEventHandle;
        base.OnStop();
    }

    protected override State OnUpdate()
    {
        if (_remainCnt > 0)
        {
            return State.RUNNING;
        }
        return State.SUCCESS;
    }
}
