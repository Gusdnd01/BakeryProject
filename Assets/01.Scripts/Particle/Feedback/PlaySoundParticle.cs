using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Particle.Trigger;

public class PlaySoundParticle : ParticleTriggerEventBase
{
    [SerializeField]
    private AudioClip hitSound;
    public override void Action(ref ParticleSystem.Particle p, Collider2D col)
    {
        SoundManager.PlayAudioRandPitch(hitSound);
    }
}
