using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using System;

public enum CardProductionType
{
    Hover,
    Select
}

public class CardProductionMaster : MonoBehaviour 
{
    private List<CardProductionRecord> _recordList = new();

    private Dictionary<CardProductionType, Action<Transform>> _toPlayActionDic = new();
    private Dictionary<CardProductionType, Action<Transform>> _toQuitActionDic = new();
    [SerializeField] private float _onTweeningEasingTime;

    [Header("카드 선택")]
    [SerializeField] private float _onSelectScaleValue;
    [SerializeField] private float _shadowMovingValue;
    [SerializeField] private float _shadowAppearTime;

    [Header("카드 아이들")]
    private List<CardBase> _onHandCardList = new List<CardBase>();
    private float _onPointerInCardValue = 1;

    private void Start()
    {
        foreach(CardProductionType type in Enum.GetValues(typeof(CardProductionType)))
        {
            switch (type)
            {
                case CardProductionType.Hover:
                    break;
                case CardProductionType.Select:
                    _toPlayActionDic.Add(type, OnSelectCard);
                    _toQuitActionDic.Add(type, QuitSelectCard);
                    break;
            }
        }
    }

    #region Select
    public void OnSelectCard(Transform cardTrm)
    {
        CardProductionRecord re = _recordList.Find(x => x.IsSameType(CardProductionType.Select, cardTrm));
        re?.Kill();
        _recordList.Remove(re);

        cardTrm.localScale = Vector3.one;

        cardTrm.rotation = Quaternion.identity;

        RectTransform cardTransform = cardTrm as RectTransform;
        cardTransform.SetAsLastSibling();

        Tween onSelectTween = 
        cardTrm.DOScale(cardTrm.localScale * _onSelectScaleValue, _onTweeningEasingTime).
        SetEase(Ease.OutBounce);

        CardProductionRecord record = new CardProductionRecord(CardProductionType.Select, cardTrm);
        _recordList.Add(record);
    }
    public void QuitSelectCard(Transform cardTrm)
    {
        CardProductionRecord re = _recordList.Find(x => x.IsSameType(CardProductionType.Select, cardTrm));
        re?.Kill();
        _recordList.Remove(re);

        Tween outSelectTween = 
        cardTrm.DOScale(Vector3.one, _onTweeningEasingTime).
        SetEase(Ease.OutBounce);

        CardProductionRecord record = new CardProductionRecord(CardProductionType.Select, cardTrm);
        _recordList.Add(record);
    }
    #endregion

    #region CardIdle
    public void OnCardIdling(CardBase cardBase)
    {
        cardBase.CardIdlingAddValue = UnityEngine.Random.Range(-4, 4);
        _onHandCardList.Add(cardBase);
    }
    public void QuitCardling(CardBase cardBase)
    {
        _onHandCardList.Remove(cardBase);
    }
    #endregion

    private void Update()
    {
        foreach(var card in _onHandCardList)
        {
            if (CardReader.OnPointerCard == card || !card.CanUseThisCard) continue;

            float sineX = Mathf.Sin(Time.time + card.CardIdlingAddValue);
            float cosineY = Mathf.Cos(Time.time + card.CardIdlingAddValue);

            card.VisualTrm.localEulerAngles = new Vector3(sineX, cosineY, 0) * 10;
        }
    }
}
