using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    public void ApplyDamage(int damage, Entity dealer, Action action);

    //상태이상 걸기
    public void SetAilment(AilmentEnum ailment, int duration);
}