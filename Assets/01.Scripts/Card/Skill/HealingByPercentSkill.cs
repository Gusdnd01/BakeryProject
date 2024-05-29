using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingByPercentSkill : CardBase, ISkillEffectAnim
{
    public int healingAmount;

    public override void Abillity()
    {
        IsActivingAbillity = true;
        Player.UseAbility(this);
        Player.OnAnimationCall += HandleAnimationCall;
        Player.VFXManager.OnEndEffectEvent += HandleEffectEnd;
    }

    public void HandleAnimationCall()
    {
        Player.VFXManager.PlayParticle(CardInfo, (int)CombineLevel);
        StartCoroutine(HealCor());
        Player.OnAnimationCall -= HandleAnimationCall;
    }

    public void HandleEffectEnd()
    {
        Player.EndAbility();
        Player.VFXManager.EndParticle(CardInfo, (int)CombineLevel);
        IsActivingAbillity = false;
        Player.VFXManager.OnEndEffectEvent -= HandleEffectEnd;
    }

    private IEnumerator HealCor()
    {
        yield return new WaitForSeconds(0.3f);

        Player.HealthCompo.ApplyHeal(Mathf.RoundToInt(Player.HealthCompo.maxHealth * healingAmount * 0.01f));
    }
}
