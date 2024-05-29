using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CostCheck : MonoBehaviour
{
    [SerializeField] private GameObject _costObject;
    private Tween _numberingTween;
    private int _targetCost;
    [SerializeField] private TextMeshProUGUI _costText;
    [SerializeField] private Image[] _extramanaArr;

    private void Start()
    {
        CostCalculator.MoneyChangeEvent += HandleCheckCost;
        HandleCheckCost(CostCalculator.CurrentMoney);

        CostCalculator.ExtraManaChangeEvent += HandleCheckExMana;
        HandleCheckExMana(CostCalculator.CurrentExMana);

        TurnCounter.PlayerTurnStartEvent += HandleCalculateExMana;
        TurnCounter.PlayerTurnStartEvent += HandleEnableCostObj;

        TurnCounter.PlayerTurnEndEvent += HandleDisableCostObj;
    }

    private void OnDisable()
    {
        CostCalculator.MoneyChangeEvent -= HandleCheckCost;
        CostCalculator.ExtraManaChangeEvent -= HandleCheckExMana;
        TurnCounter.PlayerTurnStartEvent -= HandleEnableCostObj;
        TurnCounter.PlayerTurnEndEvent -= HandleDisableCostObj;
    }

    private void HandleEnableCostObj(bool  vlaue)
    {
        _costObject.SetActive(true);
    }

    private void HandleDisableCostObj()
    {
        _costObject.SetActive(false);
    }

    private void HandleCalculateExMana(bool a)
    {
        CostCalculator.GetExMana(CostCalculator.CurrentMoney);
        CostCalculator.CurrentMoney = 10;
        CostCalculator.GetCost(0);
    }

    private void HandleCheckCost(int currentMoney)
    {
        _numberingTween.Kill();
        _targetCost = currentMoney;

        _costText.transform.DOScale(Vector3.one * 1.2f, 0.2f).OnComplete(() =>
        _costText.transform.DOScale(Vector3.one, 0.2f));

        int currentMarkingNum = Convert.ToInt16(_costText.text);
        _numberingTween = DOTween.To(() => currentMarkingNum, 
                                      m => _costText.text = m.ToString(), 
                                      _targetCost, 0.5f);
    }

    private void HandleCheckExMana(int currentMana)
    {
        for (int i = 0; i < _extramanaArr.Length; i++)
        {
            //_extramanaArr[i].enabled = false;
        }

        for (int i = 0; i < currentMana; i++)
        {
            //_extramanaArr[i].enabled = true;
        }
    }
}
