using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public struct ParticleEvent
{
    [Serializable]
    public struct ParticleEventData
    {
        public float time;
        public UnityEvent func;
    }
    public event Action OnStartEvnet;
    public List<ParticleEventData> OnTimeEvent;
    public event Action OnEndEvnet;

    public ParticleEvent(Action startEvent, Action endEvent)
    { 
        OnStartEvnet = startEvent;
        OnTimeEvent = new();
        OnEndEvnet = endEvent;
    }

    public void InvokeStartEvent()
    {
        OnStartEvnet?.Invoke();
    }
    public void InvokeEndEvent()
    {
        OnEndEvnet?.Invoke();
    }
}
