using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DungeonFireUIFX : MonoBehaviour
{
    private RectTransform rtTrm;
    private ParticleSystem _fx;

    private void Awake()
    {
        TryGetComponent<ParticleSystem>(out _fx);
        rtTrm = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        _fx.Play();
    }

    private void OnDisable()
    {
        _fx.Stop();
    }

    //DEBUG
    /*private void Update()
    {
        if (Keyboard.current.wKey.wasPressedThisFrame)
        {
            Play();
        }
        if (Keyboard.current.wKey.wasReleasedThisFrame)
        {
            Stop();
        }
    }*/
}
