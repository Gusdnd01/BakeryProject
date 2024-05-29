using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingBuff : SpecialBuff
{
    public List<int> healingAmounts;
    public int turnDuration;

    public override void Active(int level)
    {
        base.Active(level);
        entity.HealthCompo.ApplyHeal(Mathf.RoundToInt(entity.HealthCompo.maxHealth * healingAmounts[combineLevel] * 0.01f));
        turnDuration--;

        if(turnDuration <= 0)
        {
            SetIsComplete(true);
        }
    }
}
