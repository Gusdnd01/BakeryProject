using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineBattleProduction : BattleProduction
{
    [SerializeField] private GameObject _turnSystem;
    private void Start()
    {
        _playerAppear = FindObjectOfType<PlayerAppear>();
    }

    public void StartBattle()
    {
        UIManager.Instance.GetSceneUI<MineUI>().PanelActive(false);
        _turnSystem.SetActive(true);
        StartCoroutine(ProductionCo());
    }
}
