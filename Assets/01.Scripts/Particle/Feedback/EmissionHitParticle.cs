using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Particle.Trigger;

[System.Serializable]
public struct EmissionParticleInfo
{
    public ParticleSystem particle;
    public int emitCount;
}

public class EmissionHitParticle : ParticleTriggerEventBase
{
    public List<EmissionParticleInfo> particles;
    public override void Action(ref ParticleSystem.Particle p, Collider2D col)
    {
        foreach (var particleInfo in particles)
        {
            ParticleSystem.MainModule mainModule = particleInfo.particle.main;
            foreach (var health in info.Targets)
            {
                ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams();
                emitParams.applyShapeToPosition = true;
                particleInfo.particle.gameObject.SetActive(true);
                emitParams.applyShapeToPosition = true;
                emitParams.position = particleInfo.particle.transform.InverseTransformPoint((Vector3)Random.insideUnitCircle * 0.1f + health.transform.position);
                emitParams.startSize = Random.Range(mainModule.startSize.constantMin, mainModule.startSize.constantMax);
                particleInfo.particle.Emit(emitParams, particleInfo.emitCount);
            }
            particleInfo.particle.Play();
        }
    }
}

