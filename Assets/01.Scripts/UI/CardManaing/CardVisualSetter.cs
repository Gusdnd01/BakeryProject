using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardVisualSetter : CardSetter
{
    [SerializeField] private TextMeshProUGUI _cardCostText;
    [SerializeField] private TextMeshProUGUI _cardNameText;
    [SerializeField] private Image _cardVisual;

    public override void SetCardInfo(CardShameElementSO shameData, CardInfo cardInfo, int combineLevel)
    {
        _cardCostText.text = CardManagingHelper.GetCardShame(shameData, CardShameType.Cost, combineLevel - 1).ToString();
        _cardVisual.sprite = cardInfo.CardVisual;
    }
}
