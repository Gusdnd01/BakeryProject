using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CardDefine;

public class SkillManager : MonoSingleton<SkillManager>
{
    private Dictionary<Type, Skill> _skills;
    private Dictionary<PlayerSkill, Type> _skillTypeDictionary;

    private void Awake()
    {
        _skills = new Dictionary<Type, Skill>();
        _skillTypeDictionary = new Dictionary<PlayerSkill, Type>();

        foreach (PlayerSkill skill in Enum.GetValues(typeof(PlayerSkill)))
        {
            Skill skillComponent = GetComponent($"{skill.ToString()}Skill") as Skill;
            Type skillType = skillComponent.GetType();
            _skills.Add(skillType, skillComponent);
            _skillTypeDictionary.Add(skill, skillType);
        }
    }
    public T GetSkill<T>() where T : Skill
    {
        Type t = typeof(T);
        if (_skills.TryGetValue(t, out Skill targetSkill))
        {
            return targetSkill as T;
        }
        return null;
    }

    public Skill GetSkill(PlayerSkill enumType)
    {
        Type skillType = _skillTypeDictionary[enumType];
        if (_skills.TryGetValue(skillType, out Skill targetSkill))
        {
            return targetSkill;
        }
        else
            return null;
    }

    public void UseSkillFeedback(PlayerSkill skill)
    {

    }
}
