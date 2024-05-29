using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TripleStepSkill : CardBase, ISkillEffectAnim
{
    private Color minimumAlphaColor = new Color(255, 255, 255, 0.1f);
    private Color maximumAlphaColor = new Color(255, 255, 255, 1);

    private List<Entity> target = new();

    public override void Abillity()
    {
        IsActivingAbillity = true;
        target = Player.GetSkillTargetEnemyList[this];
        Player.UseAbility(this, true);
        Player.OnAnimationCall += HandleAnimationCall;
        Player.VFXManager.OnEndEffectEvent += HandleEffectEnd;

        foreach (var m in battleController.onFieldMonsterList)
        {
            if (target.Contains(m)) continue;
            m.SpriteRendererCompo.DOColor(minimumAlphaColor, 0.5f);
        }
    }

    public void HandleAnimationCall()
    {
        Player.VFXManager.PlayParticle(CardInfo, Player.forwardTrm.position + new Vector3(1.8f, 0f, 0f), (int)CombineLevel);
        if (target.Count > 0)
            StartCoroutine(AttackCor());
        Player.OnAnimationCall -= HandleAnimationCall;
    }

    public void HandleEffectEnd()
    {
        Player.EndAbility();
        Player.MoveToOriginPos();
        Player.VFXManager.EndParticle(CardInfo, (int)CombineLevel);
        IsActivingAbillity = false;
        Player.VFXManager.OnEndEffectEvent -= HandleEffectEnd;

        foreach (var m in battleController.onFieldMonsterList)
        {
            if (target.Contains(m)) continue;
            m.SpriteRendererCompo.DOColor(maximumAlphaColor, 0.5f);
        }
    }

    private IEnumerator AttackCor()
    {
        for (int i = 0; i < 2; ++i)
        {
            yield return new WaitForSeconds(0.2f);
            Player.target.HealthCompo.ApplyDamage(GetDamage(CombineLevel)[0], Player);

            if (Player.target != null)
            {
                GameObject fx = Instantiate(CardInfo.hitEffect.gameObject, Player.target.transform.position, Quaternion.identity);
                Destroy(fx, 1.0f);

                FeedbackManager.Instance.ShakeScreen(new Vector3(-.25f, .25f, 0));
            }
        }

        yield return new WaitForSeconds(2.6f);

        Player.target.HealthCompo.ApplyDamage(GetDamage(CombineLevel)[0] * 2, Player);

        GameObject obj = Instantiate(CardInfo.hitEffect.gameObject, Player.target.transform.position, Quaternion.identity);
        Destroy(obj, 1.0f);
        FeedbackManager.Instance.ShakeScreen(2);
        FeedbackManager.Instance.EndSpeed = 2.0f;
    }
}
