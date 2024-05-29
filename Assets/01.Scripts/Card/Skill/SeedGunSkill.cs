using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeedGunSkill : CardBase, ISkillEffectAnim
{
    private float yPos;

    public override void Abillity()
    {
        IsActivingAbillity = true;

        yPos = Player.transform.position.y;
        if (Player.target == null) Player.UseAbility(this);
        else Player.transform.DOMoveY(Player.target.transform.position.y, 0.1f).OnComplete(() => Player.UseAbility(this));
        Player.OnAnimationCall += HandleAnimationCall;
        Player.VFXManager.OnEndEffectEvent += HandleEffectEnd;

        foreach (var e in battleController.onFieldMonsterList)
        {
            if (Player.GetSkillTargetEnemyList[this].Contains(e)) continue;
            e.SpriteRendererCompo.DOColor(minimumColor, 0.5f);
        }
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
        Player.transform.DOMoveY(yPos, 0.1f).OnComplete(() =>
        {
            IsActivingAbillity = false;
        });
        Player.VFXManager.OnEndEffectEvent -= HandleEffectEnd;

        foreach (var e in battleController.onFieldMonsterList)
        {
            if (e == null) continue;
            e.SpriteRendererCompo.DOColor(maxtimumColor, 0.5f);

        }
    }
}
