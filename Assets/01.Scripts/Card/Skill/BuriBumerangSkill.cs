using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuriBumerangSkill : CardBase, ISkillEffectAnim
{
    public override void Abillity()
    {
        IsActivingAbillity = true;

        Player.UseAbility(this);
        Player.OnAnimationCall += HandleAnimationCall;
        Player.VFXManager.OnEndEffectEvent += HandleEffectEnd;
    }

    public void HandleAnimationCall()
    {
        Player.VFXManager.PlayParticle(this, Player.forwardTrm.position);
        Player.OnAnimationCall -= HandleAnimationCall;
    }

    public void HandleEffectEnd()
    {
        Player.EndAbility();
        Player.VFXManager.EndParticle(CardInfo, (int)CombineLevel);
        IsActivingAbillity = false;
        Player.VFXManager.OnEndEffectEvent -= HandleEffectEnd;
    }

    private IEnumerator AttackCor()
    {
        yield return new WaitForSeconds(0.3f);

        foreach (var e in Player.GetSkillTargetEnemyList[this])
        {
            e.HealthCompo.ApplyDamage(GetDamage(CombineLevel)[0], Player);
        }

        yield return new WaitForSeconds(1.2f);

        foreach (var e in Player.GetSkillTargetEnemyList[this])
        {
            e.HealthCompo.ApplyDamage(GetDamage(CombineLevel)[0], Player);
        }
    }
}
