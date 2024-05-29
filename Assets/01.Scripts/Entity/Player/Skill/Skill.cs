using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CardDefine;

public delegate void CooldownNotify(float current, float total);

public class Skill : MonoBehaviour
{
    public bool skillEnabled = false;

    [SerializeField] protected LayerMask _whatisEnemy;
    [SerializeField] protected float _cooldown;
    protected float _cooldownTiemr;
    //protected Player _player => GameManager.Instance.Player;

    [SerializeField] protected PlayerSkill _skillType;

    public event CooldownNotify OnCooldown;

    protected virtual void Update()

    {
        if(_cooldownTiemr > 0)
        {
            _cooldownTiemr -= Time.deltaTime;
            if(_cooldownTiemr <= 0)
            {
                _cooldownTiemr = 0;
            }

            OnCooldown?.Invoke(_cooldownTiemr, _cooldown);
        }
    }
    public virtual bool AttemptUseSkill()
    {
        if(_cooldownTiemr <= 0 && skillEnabled)
        {
            _cooldownTiemr = _cooldown;
            UseSkill();
            return true;
        }
        Debug.Log("Skill cooldown or locked");
        return false;
    }

    public virtual void UseSkill()
    {
        SkillManager.Instance.UseSkillFeedback(_skillType);
    }
    public virtual void UseSkillWithoutCooltimeAndEffect()
    {

    }

    public virtual Transform FindClosestEnemy(Transform checkTransform, float radius)
    {
        Transform targetEneny = null;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(checkTransform.position, radius, _whatisEnemy);
        float closestDistance = Mathf.Infinity;

        foreach (Collider2D collider in colliders)
        {
            float distance = Vector2.Distance(checkTransform.position, collider.transform.position);
            if(distance <closestDistance)
            {
                closestDistance = distance;
                targetEneny = collider.transform;
            }
        }
        return targetEneny;
    }
}
