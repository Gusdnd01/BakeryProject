using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleUI : SceneUI
{
    [SerializeField] private BattleController _battleController;
    public bool IsBattleEnd => _battleController.IsGameEnd;

    [SerializeField] private BattleResultPanel _battleResultPanel;
    [SerializeField] private TurnCounting _turnCounting;
    [SerializeField] private GameObject _battleSystem;

    public Action<bool> SystemActive { get; private set; }

    public override void SceneUIStart()
    {
        SystemActive += HandleBattleSystemActive;
    }

    public override void SceneUIEnd()
    {
        SystemActive -= HandleBattleSystemActive;
    }

    protected void HandleBattleSystemActive(bool isActive)
    {
        _battleSystem.SetActive(isActive);
    }

    public void SetResult(bool isClear)
    {
        Sequence seq = _turnCounting.BattleEndSequence(isClear);
        seq.AppendCallback(() =>
        {
            _battleResultPanel.gameObject.SetActive(true);
            StageDataSO currentStage = MapManager.Instanace.SelectStageData;
            _battleResultPanel.LookResult(isClear,
                                          currentStage.stageType,
                                          currentStage.stageName,
                                          currentStage.clearCondition.Info);
        });
    }
}
