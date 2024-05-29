using Particle;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyVFXPlayer : MonoBehaviour
{
    public event Action OnEndEffect;

    public void PlayParticle(EnemyAttack enemyAttack)
    {
        enemyAttack.attack.gameObject.SetActive(true);
        enemyAttack.attack.StartParticle(null,OnEndEffect);
    }
    public void SetTarget()
    {

    }
}
