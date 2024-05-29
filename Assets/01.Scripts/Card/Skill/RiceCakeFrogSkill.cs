using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiceCakeFrogSkill : CardBase
{
    public override void Abillity()
    {
        IsActivingAbillity = true;
        Player.VFXManager.PlayParticle(CardInfo, (int)CombineLevel);
        Player.UseAbility(this);
        StartCoroutine(WaitSpawnCor());
    }
    private IEnumerator WaitSpawnCor()
    {
        yield return new WaitForSeconds(0.1f);
        Player.EndAbility();
        Player.BuffStatCompo.AddBuff(buffSO, 0,(int)CombineLevel);
        Player.VFXManager.EndParticle(CardInfo, (int)CombineLevel);
        IsActivingAbillity = false;
    }
}
