using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class SelectToManagingCardElement : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image _visual;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _costText;
    [SerializeField] private GameObject _usingMask;

    public CardInfo CardInfo { get; private set; }
    public Action UnSelectedAction { get; set; }

    public bool IsSelect
    {
        set
        {
            UnSelectedAction?.Invoke();
            _usingMask.SetActive(value);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        IsSelect = true;
        UIManager.Instance.GetSceneUI<CardManagingUI>().OnSelectToManagingCard(this);
    }

    public void UnSelectCard()
    {
        IsSelect = false;
    }

    public void SetInfo(CardInfo info)
    {
        CardInfo = info;

        _visual.sprite = info.CardVisual;
        _nameText.text = info.CardName;
        _costText.text = CardManagingHelper.GetCardShame(info.cardShameData, CardShameType.Cost, 1).ToString();
    }
}
