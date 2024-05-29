using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleBossBuff : SpecialBuff, IOnHitDamageAfter
{
    public int durationTurn = 5;
    private int _remainTurn = 0;

    private float _targetHealthAmount = 1f;

    private int totalDmg = 0;
    public override void Active(int level)
    {
        base.Active(level);
        _remainTurn--;
        if (_remainTurn <= 0)
        {
            if (entity.HealthCompo.GetNormalizedHealth() > _targetHealthAmount)
            {
                Debug.Log("∫Œ¡∑«— µÙ∑Æ!");
                entity.target.HealthCompo.ApplyDamage(Mathf.RoundToInt(totalDmg * 0.5f), entity);
                FeedbackManager.Instance.ShakeScreen(3f);
                entity.HealthCompo.ApplyHeal(Mathf.RoundToInt(totalDmg * 0.5f));
            }
            SetIsComplete(true);
        }
    }
    public override void Init()
    {
        _remainTurn = durationTurn;
        totalDmg = 0;
        _targetHealthAmount = Mathf.Clamp(entity.HealthCompo.GetNormalizedHealth() - 0.3f, 0f, 1f);
    }
    public override void SetIsComplete(bool value)
    {
        Init();
    }

    public void HitDamageAfter(Entity dealer, Health health, ref int damage)
    {
        totalDmg += damage;
    }
}
