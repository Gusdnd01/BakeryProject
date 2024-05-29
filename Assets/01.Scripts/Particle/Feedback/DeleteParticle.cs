using Particle.Trigger;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteParticle : ParticleTriggerEventBase
{
    public override void Action(ref ParticleSystem.Particle p, Collider2D col)
    {
        p.remainingLifetime = -.1f;
    }
}

