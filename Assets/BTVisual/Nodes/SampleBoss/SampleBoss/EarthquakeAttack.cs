using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BTVisual;

public class EarthquakeAttack : SampleBossNode, IAnimationEventHandler,IAnimationEndHandler
{
    private readonly int _earthquakeAnimHash = Animator.StringToHash("EarthQuake");

    [SerializeField] private int _quakeCnt = 3;
    private int _remainCnt;

    public void OnAnimationEndHandle()
    {

    }

    public void OnAnimationEventHandle()
    {
        brain.target.HealthCompo.ApplyDamage(Mathf.RoundToInt(brain.CharStat.GetDamage() * 0.2f), brain);
        FeedbackManager.Instance.ShakeScreen(2f);
        if (--_remainCnt > 0) brain.AnimatorCompo.SetTrigger(_earthquakeAnimHash);

    }

    protected override void OnStart()
    {
        base.OnStart();

        _remainCnt = _quakeCnt;
        brain.AnimatorCompo.SetTrigger(_earthquakeAnimHash);

        brain.BossAnimator.OnAnimationEvent += OnAnimationEventHandle;
        brain.BossAnimator.OnAnimationEnd += OnAnimationEndHandle;
    }

    protected override void OnStop()
    {
        brain.BossAnimator.OnAnimationEvent -= OnAnimationEventHandle;
        brain.BossAnimator.OnAnimationEnd -= OnAnimationEndHandle;

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
