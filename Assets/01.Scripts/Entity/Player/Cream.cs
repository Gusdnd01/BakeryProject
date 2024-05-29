using System;
using UnityEngine;
using DG.Tweening;

public class Cream : MonoBehaviour
{
    public Animator animator;

    public Action OnAnimationCall;
    public Action OnAnimationEnd;

    [SerializeField] private ParticleSystem particle;

    public void InvokeAnimationCall()
    {
        particle.Play();
        OnAnimationCall?.Invoke();
    }
    public void InvokeAnimationEnd()
    {
        OnAnimationEnd?.Invoke();
    }
}
