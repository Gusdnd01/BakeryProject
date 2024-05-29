using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiceCakeFrogBuff : SpecialBuff, IOnHitDamage
{
    public int hitCount = 2;
    [Range(0, 100)]
    public List<int> healAmounts;

    public Enemy hitEnemy;

    public bool isActive;

    public override void EndBuff()
    {
    }

    public void HitDamage(Entity dealer, ref int damage)
    {
        if (isActive) return;

        entity.HealthCompo.MakeInvincible(true);

        Enemy e = dealer as Enemy;
        hitEnemy = e;
        e.OnAttackEnd += HandleEndEnemyAttack;

        hitCount--;
        int amount = Mathf.RoundToInt(entity.HealthCompo.maxHealth * healAmounts[combineLevel] * 0.01f);
        entity.HealthCompo.ApplyHeal(amount);

        isActive = true;

        SetIsComplete(hitCount <= 0);
    }
    private void HandleEndEnemyAttack()
    {
        isActive = false;
        entity.HealthCompo.MakeInvincible(false);
        hitEnemy.OnAttackEnd -= HandleEndEnemyAttack;
    }
}
