using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using TMPro;

public class SelectedDeck : MonoBehaviour,IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image _cardPreLook;
    [SerializeField] private float _normalPosY;
    [SerializeField] private float _upperPosY;
    [SerializeField] private TextMeshProUGUI _deckNameText;
    public List<CardBase> SelectDeck { get; private set; }
    private Tween _lookPreCardTween;

    public void OnPointerEnter(PointerEventData eventData)
    {
        _lookPreCardTween.Kill();
        _lookPreCardTween = _cardPreLook.transform.DOLocalMoveY(_upperPosY, 0.2f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _lookPreCardTween.Kill();
        _lookPreCardTween = _cardPreLook.transform.DOLocalMoveY(_normalPosY, 0.2f);
    }

    public void SetDeckInfo(string deckName, List<CardBase> deck)
    {
        SelectDeck = deck;
        _cardPreLook.sprite = deck[deck.Count - 1].CardInfo.CardVisual;
        _deckNameText.text = deckName;
    }
}
