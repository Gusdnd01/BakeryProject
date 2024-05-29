using ExtensionFunction;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Linq;

public class CardShameSetter : CardSetter
{
    [SerializeField] private Sprite[] _shameIconSprite;
    [SerializeField] private string[] _shameTypeName;

    [SerializeField] private Transform _cardShameTrm;
    [SerializeField] private CardShameElement _cardShameElement;
    [SerializeField] private TextMeshProUGUI _gookBapLevelText;

    public (Sprite, string, int, int, string) GetShameDataGroup(CardShameElementSO data, CardShameType type, int combineLevel)
    {
        CardShameData shameData = CardManagingHelper.GetCardShameData(data, type, combineLevel);

        int shameType = (int)(Mathf.Log((int)shameData.cardShameType) / Mathf.Log(2));

        return (_shameIconSprite[shameType], _shameTypeName[shameType],
                 CardManagingHelper.GetAfterLevelShame(data, shameData.cardShameType, combineLevel), 
                 CardManagingHelper.GetCardShame(data, shameData.cardShameType, combineLevel), 
                 shameData.info);
    }

    private (float, float) GetGookBapShame(CardShameElementSO data, int combineLevel)
    {
        int currentShame = CardManagingHelper.GetCardShame(data, CardShameType.Damage |
                                                                 CardShameType.Buff |
                                                                 CardShameType.Debuff,
                                                                 combineLevel);

        int cost = CardManagingHelper.GetCardShame(data, CardShameType.Cost, combineLevel);

        int targetCount = CardManagingHelper.GetCardShame(data, CardShameType.Range, combineLevel);

        int afterShame = CardManagingHelper.GetAfterLevelShame(data, CardShameType.Damage |
                                                                     CardShameType.Buff |
                                                                     CardShameType.Debuff,
                                                                     combineLevel);

        int aftercost = CardManagingHelper.GetAfterLevelShame(data, CardShameType.Cost, combineLevel);
        int aftertargetCount = CardManagingHelper.GetAfterLevelShame(data, CardShameType.Range, combineLevel);

        return ((currentShame * targetCount / cost), (afterShame * aftertargetCount / aftercost));
    }

    public void SetGookBapShame(CardShameElementSO data, int combineLevel)
    {
        (float, float) gookPower = GetGookBapShame(data, combineLevel);

        _gookBapLevelText.text = 
        $"{gookPower.Item1} -> <color=#37B1FF>{gookPower.Item2}<size=25> (+{gookPower.Item2 - gookPower.Item1})</size></color>";
    }

    public void LevelUpCardShame(CardShameElementSO selectShameData)
    {
        //selectShameData.cardShameDataList
    }

    public override void SetCardInfo(CardShameElementSO shameData, CardInfo cardInfo, int combineLevel)
    {
        combineLevel = Mathf.Clamp(combineLevel - 1, 0, 5);
        List<CardShameData> dataList = shameData.cardShameDataList[combineLevel].list[shameData.cardLevel - 1].list;

        SetGookBapShame(shameData, combineLevel);

        _cardShameTrm.Clear();
        foreach (CardShameData data in dataList)
        {
            CardShameElement cse = Instantiate(_cardShameElement, _cardShameTrm);
            cse.SetShame(GetShameDataGroup(shameData, data.cardShameType ,combineLevel));
        }
    }
}
