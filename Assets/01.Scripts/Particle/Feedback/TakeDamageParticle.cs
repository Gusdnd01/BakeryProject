using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Particle.Trigger;
public class TakeDamageParticle : ParticleTriggerEventBase
{
    public override void Action(ref ParticleSystem.Particle p, Collider2D col)
    {
        foreach (var d in info.Damages)
        {
            foreach (var t in info.Targets)
            {
                t.HealthCompo.ApplyDamage(d, info.Owner);
            }
        }
    }
}