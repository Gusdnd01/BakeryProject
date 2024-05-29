using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IOnTakeDamage
{
    public void TakeDamage(Health health,ref int dmg);
}
