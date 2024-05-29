using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaesalhariSkill : ChilledCardBase, ISkillEffectAnim
{
    public override void Abillity()
    {
        IsActivingAbillity = true;
        Player.UseAbility(this);
        Player.VFXManager.PlayParticle(CardInfo, (int)CombineLevel);
        Player.VFXManager.OnEndEffectEvent += HandleEffectEnd;
        Player.BuffStatCompo.AddBuff(buffSO,2,(int)CombineLevel);
    }

    public void HandleAnimationCall()
    {

    }

    public void HandleEffectEnd()
    {
        Player.VFXManager.OnEndEffectEvent -= HandleEffectEnd;
        Player.EndAbility();
        Player.VFXManager.EndParticle(CardInfo, (int)CombineLevel);
        IsActivingAbillity = false;
    }
}   
