using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrindingSkill : CardBase, ISkillEffectAnim
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
        Player.VFXManager.PlayParticle(CardInfo, (int)CombineLevel);
        StartCoroutine(AddStackCor());
        Player.OnAnimationCall -= HandleAnimationCall;
    }

    public void HandleEffectEnd()
    {
        Player.EndAbility();
        Player.VFXManager.EndParticle(CardInfo, (int)CombineLevel + 1);
        IsActivingAbillity = false;
        Player.VFXManager.OnEndEffectEvent -= HandleEffectEnd;
    }

    private IEnumerator AddStackCor()
    {
        yield return new WaitForSeconds(0.3f);

        Player.BuffStatCompo.AddStack(StackEnum.Forging, (int)CombineLevel);
        Player.BuffStatCompo.AddBuff(buffSO, 2, (int)CombineLevel);

        Debug.Log($"Current Forging Stat: {Player.BuffStatCompo.GetStack(StackEnum.Forging)}");
    }
}
