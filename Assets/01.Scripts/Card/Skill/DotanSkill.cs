using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DotanSkill : CardBase, ISkillEffectAnim
{
    public override void Abillity()
    {
        IsActivingAbillity = true;
        Player.UseAbility(this);
        Player.OnAnimationCall += () => Player.VFXManager.PlayParticle(CardInfo, (int)CombineLevel);
        Player.OnAnimationEnd += () => IsActivingAbillity = false;
    }

    public void HandleAnimationCall()
    {

    }

    public void HandleEffectEnd()
    {

    }
}
