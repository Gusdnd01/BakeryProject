using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Particle.Trigger;
using System;

namespace Particle
{
    public class ParticleInfo : MonoBehaviour
    {
        public AudioClip soundEffect;

        private ParticleSystem ps;

        [SerializeField]private List<ParticleTriggerInfo> triggerInfos;
        //[SerializeField] private TakeDamageParticle[] takeDamages;

        public float duration;

        private List<Entity> _targets = new();
        public int[] damages { get; set; }
        public Entity owner { get; set; }

        private void Awake()
        {
            ps = GetComponent<ParticleSystem>();
        }
        private void Start()
        {
            foreach (ParticleTriggerInfo i in triggerInfos)
            {
                i.Owner = owner;
                i.Damages = damages;
            }
        }

        public void OnEnable()
        {
            _targets.Clear();
        }
        public void ClearTarget()
        {
            foreach (var col in triggerInfos)
            {
                col.ClearCollision();
            }
        }
        public void RemoveTriggerTarget(Entity target)
        {
            if (!_targets.Contains(target)) return;
            _targets.Remove(target);

            foreach (var col in triggerInfos)
            {
                col.SetCollision(_targets);
            }
        }
        public void AddTriggerTarget(Entity target)
        {
            if (_targets.Contains(target)) return;
            _targets.Add(target);
            foreach (var col in triggerInfos)
            {
                col.SetCollision(_targets);
            }
        }

        public void StartParticle(Action OnStartParticleEvent, Action OnEndParticleEvent)
        {
            ps.Play();
            OnStartParticleEvent?.Invoke();
            if(soundEffect != null)
                SoundManager.PlayAudio(soundEffect);
            StartCoroutine(WaitEndParticle(OnEndParticleEvent));
        }
        public void EndParticle(Action OnEndParticleEvent)
        {
            ps.Stop();
            OnEndParticleEvent?.Invoke();
        }
        private IEnumerator WaitEndParticle(Action OnEndParticleEvent)
        {
            yield return new WaitForSeconds(duration);
            EndParticle(OnEndParticleEvent);
        }
    }
}
