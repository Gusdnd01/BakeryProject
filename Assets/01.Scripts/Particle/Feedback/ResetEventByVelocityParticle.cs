using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Particle.Trigger;

public class ResetEventByVelocityParticle : ParticleTriggerEventBase
{
    public enum velocityType
    {
        x,
        y
    }
    [SerializeField] private velocityType velType;
    private List<IUseInit> inits = new();
    private float velocity;

    private void Awake()
    {
        GetComponents<IUseInit>(inits);
    }
    public override void Action(ref ParticleSystem.Particle p, Collider2D col)
    {
        switch (velType)
        {
            case velocityType.x:
                if (p.totalVelocity.x != 0 && Mathf.Sign(velocity) != Mathf.Sign(p.totalVelocity.x))
                {
                    velocity = p.totalVelocity.x;
                    InitEvents();
                }
                break;
            case velocityType.y:
                if (p.totalVelocity.y != 0 && Mathf.Sign(velocity) != Mathf.Sign(p.totalVelocity.y))
                {
                    velocity = p.totalVelocity.y;
                    InitEvents();
                }
                break;
        }
    }
    private void InitEvents()
    {
        if (velocity == 0) return;
        print(123);
        foreach (var item in inits)
        {
            item.Init();
        }
    }
}
