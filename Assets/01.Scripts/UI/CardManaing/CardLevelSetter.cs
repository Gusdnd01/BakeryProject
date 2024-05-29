using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CardLevelSetter : CardSetter
{
    [SerializeField] private TextMeshProUGUI _cardCurrentLevelText;
    [SerializeField] private TextMeshProUGUI _cardAfterLevelText;

    [SerializeField] private GameObject _cardLevelGroup;
    [SerializeField] private GameObject _cardMaxLevelGroup;

    [SerializeField] private Slider _cardEXPGazer;

    [SerializeField] private UnityEvent<CardShameElementSO> _cardShameUpperEvent;
    [SerializeField] private UnityEvent _cardLevelUpEvent;

    private Tween _onGaigingTween;
    private float _maxEXP;
    private CardShameElementSO _selectShameData;

    [SerializeField] private float[] _magnificationOfLevelArr;

    private bool _isInCalculating;

    public override void SetCardInfo(CardShameElementSO shameData, CardInfo cardInfo, int combineLevel)
    {
        if (_isInCalculating) return;

        _cardLevelGroup.SetActive(!(shameData.cardLevel >= 5));
        _cardMaxLevelGroup.SetActive(shameData.cardLevel >= 5);

        _isInCalculating = true;
        _selectShameData = shameData;
        
        _cardCurrentLevelText.text = shameData.cardLevel.ToString();

        string afterLvText = string.Empty;
        if (shameData.cardLevel + 1 == 5)
        {
            afterLvText = "MAX";
        }
        else
        {
            afterLvText = (shameData.cardLevel + 1).ToString();
        }
        _cardAfterLevelText.text = afterLvText;
        _maxEXP = shameData.cardLevel * _magnificationOfLevelArr[shameData.cardLevel - 1];

        SetEXP(shameData.cardExp);
    }

    public void SetEXP(float currentExp)
    {
        if(_selectShameData.cardLevel >= 5)
        {
            _cardEXPGazer.value = 1;
            return;
        }

        if(currentExp >= _maxEXP)
        {
            _selectShameData.cardExp = 0;
            _cardEXPGazer.value = 0;

            _selectShameData.cardLevel += 1;
            _selectShameData.cardExp = currentExp - _maxEXP;

            _cardShameUpperEvent?.Invoke(_selectShameData);
            _cardLevelUpEvent?.Invoke();

            _maxEXP = _selectShameData.cardLevel *
                      _magnificationOfLevelArr[_selectShameData.cardLevel - 1];

            currentExp = _selectShameData.cardExp;
        }

        _onGaigingTween.Kill();

        _isInCalculating = false;
        _onGaigingTween =
        DOTween.To(() => _cardEXPGazer.value, x => _cardEXPGazer.value = x, currentExp / _maxEXP, 0.5f).SetEase(Ease.OutBack);
    }
}
