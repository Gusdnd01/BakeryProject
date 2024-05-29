using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Tsumego/PlayerDie")]
public class PlayerDieCondition : TsumegoCondition
{
    private Player _player;

    public override bool CheckCondition()
    {
        if(_player == null)
        {
            _player = FindAnyObjectByType<Player>();
        }

        return !_player.HealthCompo.IsDead;
    }
}
