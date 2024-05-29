using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;


namespace Particle.Trigger
{
    [Flags]
    public enum ParticleTriggerType
    {
        None = 0,
        Inside = 1,
        Outside = 2,
        Enter = 4,
        Exit = 8
    }
    public class ParticleTriggerInfo : MonoBehaviour
    {
        public delegate void ParticleTriggerEvent(ref ParticleSystem.Particle p, Collider2D col);

        private ParticleSystem ps;
        private ParticleSystem.TriggerModule triggerModule;

        //enum 순서에 따라 맞는 이벤트
        public ParticleTriggerEvent[] triggerEvent = new ParticleTriggerEvent[4];
        public void AddEvent(ParticleTriggerEventBase b)
        {
            ParticleTriggerType copyType = b.Type;
            for (int i = 0; i < 4; i++)
            {
                if (((1 << i) & (int)copyType) > 0)
                {
                    triggerEvent[i] += b.Action;
                }
            }
        }

        public Entity Owner { get; set; }
        public List<Entity> Targets { get; set; }
        public int[] Damages { get; set; }

        public void SetCollision(List<Entity> l)
        {
            ClearCollision();
            Targets = l;
            foreach (var item in l)
            {
                triggerModule.AddCollider(item.ColliderCompo);
            }
        }
        public void ClearCollision()
        {
            for (int i = 0; i < triggerModule.colliderCount; i++)
            {
                triggerModule.RemoveCollider(0);
            }
        }

        private void Awake()
        {
            ps = GetComponent<ParticleSystem>();
            triggerModule = ps.trigger;

            ParticleTriggerEventBase[] events = GetComponents<ParticleTriggerEventBase>();
            foreach (var e in events)
            {
                e.Init(this);
                AddEvent(e);
            }
        }


        private void OnParticleTrigger()
        {
            foreach (ParticleSystemTriggerEventType type in Enum.GetValues(typeof(ParticleSystemTriggerEventType)))
            {
                if (!IsCallEventType(type)) continue;
                List<ParticleSystem.Particle> particleList = new();
                if (type != ParticleSystemTriggerEventType.Outside)
                {
                    int chk = ps.GetTriggerParticles(type, particleList, out var colliderData);
                    for (int i = 0; i < chk; i++)
                    {
                        int c = colliderData.GetColliderCount(i);
                        for (int j = 0; j < c; j++)
                        {
                            Collider2D col = colliderData.GetCollider(i, j) as Collider2D;
                            if (col)
                            {
                                ParticleSystem.Particle p = particleList[i];
                                triggerEvent[(int)type]?.Invoke(ref p, col);
                                particleList[i] = p;
                            }
                        }
                    }
                }
                else
                {
                    int chk = ps.GetTriggerParticles(type, particleList);
                    for (int i = 0; i < chk; i++)
                    {
                        for (int j = 0; j < triggerModule.colliderCount; j++)
                        {
                            ParticleSystem.Particle p = particleList[i];
                            triggerEvent[(int)type]?.Invoke(ref p, null);
                            particleList[i] = p;
                        }
                    }
                }
                ps.SetTriggerParticles(type, particleList);
            }
        }
        private void OnParticleSystemStopped()
        {

        }
        private bool IsCallEventType(ParticleSystemTriggerEventType type)
        {
            switch (type)
            {
                case ParticleSystemTriggerEventType.Inside:
                    return triggerModule.inside == ParticleSystemOverlapAction.Callback;
                case ParticleSystemTriggerEventType.Outside:
                    return triggerModule.outside == ParticleSystemOverlapAction.Callback;
                case ParticleSystemTriggerEventType.Enter:
                    return triggerModule.enter == ParticleSystemOverlapAction.Callback;
                case ParticleSystemTriggerEventType.Exit:
                    return triggerModule.exit == ParticleSystemOverlapAction.Callback;
                default:
                    return false;
            }
        }
    }
}

