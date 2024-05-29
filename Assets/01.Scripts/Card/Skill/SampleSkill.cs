using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SampleSkill : CardBase
{
    public override void Abillity()
    {
        StartCoroutine(SampleSkillCo());
    }

    private IEnumerator SampleSkillCo()
    {
        IsActivingAbillity = true;
        Debug.Log("This is Sample Skill");
        yield return new WaitForSeconds(2);
        IsActivingAbillity = false;
    }
}
