using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Handling : MonoBehaviour
{
    [SerializeField] private Transform _handTrm;

    [Header("핸드 포스")]
    [SerializeField] private float _normalHandYPos;
    [SerializeField] private float _downHandYPos;

    private void Start()
    {
        TurnCounter.PlayerTurnStartEvent += HandleMoveUpHand;
        TurnCounter.PlayerTurnEndEvent += HandleMoveDownHand;
    }

    private void HandleMoveUpHand(bool vlaue)
    {
        MoveHand(true);
    }

    private void HandleMoveDownHand()
    {
        MoveHand(false);
    }

    private void MoveHand(bool isDown)
    {
        float targetYPos = !isDown ? _downHandYPos : _normalHandYPos;
        _handTrm.DOLocalMoveY(targetYPos, 0.3f);
    }
}
