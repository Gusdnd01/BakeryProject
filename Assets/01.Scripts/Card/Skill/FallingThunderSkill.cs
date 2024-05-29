using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingThunderSkill : LightningCardBase, ISkillEffectAnim
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
        yield return new WaitForSeconds(0.3f);

        bool isLastEnemy = Player.GetSkillTargetEnemyList[this].Count == 1 ? true : false;

        if (isLastEnemy)
        {
            Player.VFXManager.PlayParticle(this, Player.GetSkillTargetEnemyList[this][0].transform.position);
            for (int i = 0; i < 2; ++i)
            {
                Player.GetSkillTargetEnemyList[this][0]?.HealthCompo.ApplyDamage(GetDamage(CombineLevel)[0], Player);
                if (Player.GetSkillTargetEnemyList[this][0] != null)
                {
                    GameObject obj = Instantiate(CardInfo.hitEffect.gameObject, Player.GetSkillTargetEnemyList[this][0].transform.position, Quaternion.identity);
                    Destroy(obj, 1.0f);
                }
                yield return new WaitForSeconds(0.4f);
            }
            RandomApplyShockedAilment(Player.GetSkillTargetEnemyList[this][0], 20f);
        }
        else
        {
            Player.VFXManager.PlayParticle(this, Player.transform.position);
            foreach (var e in Player.GetSkillTargetEnemyList[this])
            {
                e?.HealthCompo.ApplyDamage(GetDamage(CombineLevel)[0], Player);
                if (e != null)
                {
                    GameObject obj = Instantiate(CardInfo.hitEffect.gameObject, e.transform.position, Quaternion.identity);
                    Destroy(obj, 1.0f);
                    RandomApplyShockedAilment(e, 20f);
                }
                yield return new WaitForSeconds(0.4f);
            }
        }

        ExtraAttack();
    }
}
