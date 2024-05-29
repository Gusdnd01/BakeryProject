using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonStrowberry : Enemy
{
    protected override void Awake()
    {
        base.Awake();
    }
    protected override void Start()
    {
        base.Start();
        VFXPlayer.OnEndEffect += () => 
        {
            turnStatus = TurnStatus.End;
            OnAttackEnd?.Invoke();
        };
    }

    public override void Attack()
    {
        attackParticle.attack.AddTriggerTarget(target);

        OnAttackStart?.Invoke();

        VFXPlayer.PlayParticle(attackParticle);
        StartCoroutine(AttackCor());
    }
    private IEnumerator AttackCor()
    {
        yield return new WaitForSeconds(1.4f);
        Vector3 vfxPos = attackParticle.attack.transform.position;
        Quaternion vfxQua = attackParticle.attack.transform.rotation;
        SetSeedVFXPos();

        attackParticle.attack.transform.position = vfxPos;
        attackParticle.attack.transform.rotation = vfxQua;

    }

    public override void SlowEntityBy(float percent)
    {
    }

    public override void TurnAction()
    {
        turnStatus = TurnStatus.Running;
        Attack();
    }

    public override void TurnEnd()
    {
        OnAttackEnd?.Invoke();
        base.TurnEnd();
    }

    public override void TurnStart()
    {
        base.TurnStart();
        turnStatus = TurnStatus.Ready;
    }

    protected override void HandleEndMoveToOriginPos()
    {
    }

    protected override void HandleEndMoveToTarget()
    {
    }
    private void SetSeedVFXPos()
    {
        Vector2 pos = (Vector2)attackParticle.attack.transform.position - new Vector2(1.64f, 0);
        Vector2 dir = (Vector2)target.transform.position - pos;

        attackParticle.attack.transform.position = pos + dir.normalized;
        attackParticle.attack.transform.right = dir.normalized * -1;
    }
}
