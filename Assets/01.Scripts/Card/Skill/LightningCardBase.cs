using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LightningCardBase : CardBase
{
    [SerializeField] private ParticleSystem _shockedEffect;

    protected void ExtraAttack()
    {
        foreach (var e in battleController.onFieldMonsterList)
        {
            try
            {
                Debug.Log("번개 체인");
                e?.HealthCompo.AilmentStat.UsedToAilment(AilmentEnum.Shocked);
                //GameObject shockedEffects = Instantiate(_shockedEffect.gameObject, Player.target.transform.position, Quaternion.identity);
                //Destroy(shockedEffects, 1.0f);
            }
            catch (Exception ex)
            {
                Debug.Log(e);
            }
        }
    }

    protected void ApplyShockedAilment(Entity enemy)
    {
        enemy.HealthCompo.AilmentStat.ApplyAilments(AilmentEnum.Shocked);
    }

    protected void RandomApplyShockedAilment(Entity enemy, float percentage)
    {
        if (UnityEngine.Random.value * 100 >= percentage)
            enemy.HealthCompo.AilmentStat.ApplyAilments(AilmentEnum.Shocked);
    }
}
