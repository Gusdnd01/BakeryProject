using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectrostaticSkill : LightningCardBase, ISkillEffectAnim
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
        Player.VFXManager.PlayParticle(CardInfo, Player.transform.position, (int)CombineLevel);
        StartCoroutine(AddAilmentCor());
        Player.OnAnimationCall -= HandleAnimationCall;
    }

    public void HandleEffectEnd()
    {
        Player.EndAbility();
        Player.VFXManager.EndParticle(CardInfo, (int)CombineLevel);
        IsActivingAbillity = false;
        Player.VFXManager.OnEndEffectEvent -= HandleEffectEnd;
    }

    private IEnumerator AddAilmentCor()
    {
        yield return new WaitForSeconds(0.3f);

        foreach (var e in Player.GetSkillTargetEnemyList[this])
        {
            ApplyShockedAilment(e);
        }
    }
}
