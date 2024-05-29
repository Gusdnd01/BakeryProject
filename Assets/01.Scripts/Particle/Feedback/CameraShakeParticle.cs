using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Particle.Trigger;

public class CameraShakeParticle : ParticleTriggerEventBase
{
    public override void Action(ref ParticleSystem.Particle p, Collider2D col)
    {
        float randNumX = Random.Range(-.5f, .5f);
        float randNumY = Random.Range(-.5f, .5f);
        FeedbackManager.Instance.ShakeScreen(new Vector3(randNumX, randNumY, 0.0f));
    }
}
