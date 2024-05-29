using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OsloSpell : CardBase,ISkillEffectAnim
{
    public override void Abillity()
    {
        IsActivingAbillity = true;
        Player.UseAbility(this);
        Player.VFXManager.PlayParticle(CardInfo, (int)CombineLevel);
        Player.BuffStatCompo.AddBuff(buffSO, 0, (int)CombineLevel);
        Player.VFXManager.OnEndEffectEvent += HandleEffectEnd;
    }

    public void HandleAnimationCall()
    {
    }

    public void HandleEffectEnd()
    {
        Player.EndAbility();
        Player.VFXManager.OnEndEffectEvent -= HandleEffectEnd;
        Player.VFXManager.EndParticle(CardInfo, (int)CombineLevel);
        IsActivingAbillity = false;
    }
}
