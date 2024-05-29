using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationTriggers : MonoBehaviour
{
    private Player _player;

    private void Awake()
    {
        _player = transform.parent.GetComponent<Player>();
    }

    private void AnimationEndTrigger()
    {
        _player.OnAnimationEnd?.Invoke();
    }
    private void AnimationEventTrigger()
    {
        _player.OnAnimationCall?.Invoke();
    }
}
