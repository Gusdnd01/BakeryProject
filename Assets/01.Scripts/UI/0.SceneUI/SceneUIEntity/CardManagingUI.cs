using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CardManagingUI : SceneUI
{
    [SerializeField] private int _loadStoneCount;
    public int LoadStoneCount => _loadStoneCount;

    public CardShameElementSO CurrentCardShameElementInfo { get; private set; }
    public SelectToManagingCardElement SelectCardElement { get; private set; }

    [SerializeField] private UnityEvent<float> _onPressLevelUpEvent;
    [SerializeField] private UnityEvent<CardInfo> _onSelectToManagingCardEvent;

    public void PressLevelUpButton()
    {
        if (CurrentCardShameElementInfo.cardLevel >= 5)
        {
            ErrorText et = PoolManager.Instance.Pop(PoolingType.ErrorText) as ErrorText;
            et.Erroring("카드가 최대 레벨입니다");
            return;
        }

        int toUseGoods = CurrentCardShameElementInfo.cardLevel * 50;

        if(CanUseGoods(toUseGoods))
        {
            float currentEXP = CurrentCardShameElementInfo.cardExp += toUseGoods * 0.4f;
            _onPressLevelUpEvent?.Invoke(currentEXP);
        }
    }

    public void OnSelectToManagingCard(SelectToManagingCardElement selectCardElement)
    {
        if(SelectCardElement != null)
        {
            SelectCardElement.UnSelectCard();
        }

        SelectCardElement = selectCardElement;

        CurrentCardShameElementInfo = selectCardElement.CardInfo.cardShameData;
        _onSelectToManagingCardEvent?.Invoke(selectCardElement.CardInfo);
    }

    public bool CanUseGoods(int toUseGoods)
    {
        return LoadStoneCount >= toUseGoods;
    }
}
