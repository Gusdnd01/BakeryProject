using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardSettingController : MonoBehaviour
{
    private CardSetter[] _cardSetterArr;
    private CardShameContainer _cardShameContaner;

    [SerializeField] private TextMeshProUGUI _combineText;
    private int _combineLevel = 1;

    private CardInfo _selectCardInfo;

    public void AddCombineLevel()
    {
        if (_combineLevel == 3) return;

        _combineLevel++;
        ResetInfo();
        SetCombineText();
    }
    public void MinusCombineLevel()
    {
        if (_combineLevel == 1) return;

        _combineLevel--;
        ResetInfo();
        SetCombineText();
    }

    private void Start()
    {
        _cardSetterArr = GetComponentsInChildren<CardSetter>();
        _cardShameContaner = GetComponent<CardShameContainer>();

        SetCombineText();
    }

    private void SetCombineText()
    {
        _combineText.text = $"ÄÞ¹ÙÀÎ ·¹º§ : {_combineLevel}";
    }

    public void SetCardInfo(CardInfo cardInfo)
    {
        _selectCardInfo = cardInfo;
        ResetInfo();
    }

    public void ResetInfo()
    {
        CardSetting(_combineLevel, _selectCardInfo);
    }

    public void CardSetting(int combineLevel, CardInfo cardInfo)
    {
        CardShameElementSO cardShameData = _cardShameContaner.GetCardShameData(cardInfo);

        foreach(CardSetter cardSetter in _cardSetterArr)
        {
            cardSetter.SetCardInfo(cardShameData, cardInfo, combineLevel);
        }
    }
}
