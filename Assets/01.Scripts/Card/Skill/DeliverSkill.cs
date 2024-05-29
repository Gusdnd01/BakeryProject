using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliverSkill : CardBase, ISkillEffectAnim
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
        StartCoroutine(AttackCor());
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
        yield return new WaitForSeconds(0.5f);

        foreach (var e in Player.GetSkillTargetEnemyList[this])
        {
            e?.HealthCompo.ApplyDamage(GetDamage(CombineLevel)[0], Player);
            if (e != null)
            {
                GameObject obj = Instantiate(CardInfo.hitEffect.gameObject, e.transform.position, Quaternion.identity);
                Destroy(obj, 1.0f);
            }
        }

        yield return new WaitForSeconds(3.65f);

        foreach(var e in Player.GetSkillTargetEnemyList[this])
        {
            e?.HealthCompo.ApplyDamage(5 * Player.BuffStatCompo.GetStack(StackEnum.Forging), Player);
            if(e != null)
            {
                GameObject obj = Instantiate(CardInfo.hitEffect.gameObject, e.transform.position, Quaternion.identity);
                Destroy(obj, 1.0f);
            }
        }
    }
}
