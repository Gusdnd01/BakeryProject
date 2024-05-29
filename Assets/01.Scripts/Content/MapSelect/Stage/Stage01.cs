using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;

public class Stage01 : Stage
{
    [SerializeField] private CinemachineConfiner2D _confiner;
    [SerializeField] private Collider2D _bound;

    protected override void Start()
    {
        base.Start();
        OnPhaseCleared += ApplyConfiner;
    }

    private void ApplyConfiner()
    {
        _confiner.m_BoundingShape2D = _bound;
    }
}
