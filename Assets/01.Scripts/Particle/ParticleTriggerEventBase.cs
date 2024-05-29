using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Particle.Trigger
{
    [RequireComponent(typeof(ParticleTriggerInfo))]
    public abstract class ParticleTriggerEventBase : MonoBehaviour
    {
        public ParticleTriggerType Type;
        protected ParticleTriggerInfo info;
    
        public virtual void Init(ParticleTriggerInfo info)
        {
            this.info = info;
        }
        public abstract void Action(ref ParticleSystem.Particle p, Collider2D col);
    }
}
