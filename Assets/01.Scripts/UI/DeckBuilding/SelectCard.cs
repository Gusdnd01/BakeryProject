using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectCard : MonoBehaviour
{
    public CardInfo SelectCardInfo { get; private set; }

    [SerializeField] private Sprite _noneSelectVisual;
    [SerializeField] private GameObject _noneSelectText;
    [SerializeField] private Image _cardVisual;
    [SerializeField] private Material _cardMaterial;
    [SerializeField] private TextMeshProUGUI _costText;

    public bool IsAssignedCard { get; private set; }

    public void SetCard(CardInfo info, int combineLevel)
    {
        SelectCardInfo = info;

        _noneSelectText.SetActive(false);
        _cardVisual.sprite = info.CardVisual;
        _cardVisual.color = Color.white;
        _cardVisual.material = _cardMaterial;
        _costText.text = CardManagingHelper.GetCardShame(info.cardShameData, CardShameType.Cost, combineLevel).ToString();

        IsAssignedCard = true;
    }

    public void UnSetCard()
    {
        SelectCardInfo = null;

        _noneSelectText.SetActive(true);
        _cardVisual.sprite = _noneSelectVisual;
        _cardVisual.color = new Color(1, 1, 1, 0.8f);
        _cardVisual.material = null;
        _costText.text = string.Empty;

        IsAssignedCard = false;
    }
}
