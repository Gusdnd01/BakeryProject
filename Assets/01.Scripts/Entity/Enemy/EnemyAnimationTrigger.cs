using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationTrigger : MonoBehaviour
{
    private Enemy _enemy;

    protected virtual void Awake()
    {
        _enemy = transform.parent.GetComponent<Enemy>();
    }

    private void EndAnimationTrigger()
    {
        _enemy.OnAnimationEnd?.Invoke();
    }

    private void CallAnimationEvent()
    {
        _enemy.OnAnimationCall?.Invoke();
    }
}
