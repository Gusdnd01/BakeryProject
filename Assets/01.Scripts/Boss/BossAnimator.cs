using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BossAnimator))]
public class BossAnimator : MonoBehaviour
{
    private Animator _animator;
        
    public event Action OnAnimationEvent;
    public event Action OnAnimationEnd;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }
    public void InvokeAnimationEvent() => OnAnimationEvent?.Invoke();
    public void InvokeAnimationEnd() => OnAnimationEnd?.Invoke();
}
