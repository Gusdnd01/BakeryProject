using System;
using System.Collections.Generic;
using UnityEngine;

public enum TurnType
{
    Player,
    Enemy
}

public enum TurnStatus
{
    Ready,
    Running,
    End
}

public static class TurnCounter
{
    public static TurnType CurrentTurnType { get; private set; } = TurnType.Enemy;
    public static int TurnCount { get; private set; }
    public static int RoundCount { get; private set; }

    public static Action RoundEndEvent;
    public static Action RoundStartEvent;

    public static Action<bool> EnemyTurnStartEvent;
    public static Action EnemyTurnEndEvent;

    public static Action<bool> PlayerTurnStartEvent;
    public static Action PlayerTurnEndEvent;

    private static TurnCounting _turnCounting;
    public static TurnCounting TurnCounting
    {
        get
        {
            if(_turnCounting != null)
            {
                return _turnCounting;
            }
            _turnCounting = GameObject.FindObjectOfType<TurnCounting>();
            return _turnCounting;
        }
    }

    public static void Init()
    {
        CurrentTurnType = TurnType.Player;
        TurnCount = 0;
        RoundCount = 0;
    }

    public static void ChangeRound()
    {
        if(RoundCount != 0)
        {
            RoundEndEvent?.Invoke();
        }

        RoundCount++;
        RoundStartEvent?.Invoke();
    }

    public static void ChangeTurn()
    {
        TurnCount++;

        if(CurrentTurnType == TurnType.Player)
        {
            CurrentTurnType = TurnType.Enemy;

            PlayerTurnEndEvent?.Invoke();
            EnemyTurnStartEvent?.Invoke(false);
        }
        else
        {
            CurrentTurnType = TurnType.Player;

            EnemyTurnEndEvent?.Invoke();
            PlayerTurnStartEvent?.Invoke(false);
        }

        if(TurnCount % 2 == 0)
        {
            ChangeRound();
        }
        Debug.Log($"{TurnCount}{CurrentTurnType}");
    }
}
