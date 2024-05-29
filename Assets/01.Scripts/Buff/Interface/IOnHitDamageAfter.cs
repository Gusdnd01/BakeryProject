using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IOnHitDamageAfter 
{
    public void HitDamageAfter(Entity dealer,Health health, ref int damage);
}
