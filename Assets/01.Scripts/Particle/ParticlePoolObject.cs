using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Particle
{
    public class ParticlePoolObject : PoolableMono
    {
        [SerializeField] private List<ParticleInfo> particleSystems;
        public ParticleInfo this[int i] => particleSystems[i];
        public override void Init()
        {
        }
        private void Awake()
        {
        }
        public void Active(int combineLevel, Action OnStartParticleEvent = null, Action OnEndParticleEvent = null)
        {
            particleSystems[combineLevel].gameObject.SetActive(true);
            //particleEvent.OnStartEvnet += OnStartParticleEvent;
            particleSystems[combineLevel].StartParticle(OnStartParticleEvent, OnEndParticleEvent);
        }
    }
}
