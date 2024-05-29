using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Particle.Trigger;

public class TakeDamageOnceParticle : ParticleTriggerEventBase, IUseInit
{
    private List<Collider2D> takeDamages = new();
    public override void Action(ref ParticleSystem.Particle p, Collider2D col)
    {
        if (takeDamages.Contains(col))
            return;
        foreach (var d in info.Damages)
        {
            foreach (var t in info.Targets)
            {
                if (t.ColliderCompo == col)
                {
                    t.HealthCompo.ApplyDamage(d, info.Owner);
                    takeDamages.Add(t.ColliderCompo);
                }
            }
        }
    }

    public void Init()
    {
        takeDamages.Clear();
    }
}
