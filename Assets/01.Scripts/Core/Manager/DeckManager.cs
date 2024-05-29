using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoSingleton<DeckManager>
{
    [SerializeField] private CardBase[] _totalCardArr;
    private Dictionary<string, CardBase> _getCardDic = new();
    public (DeckElement, int) SaveDummyDeck { get; set; }

    private void Awake()
    {
        foreach(CardBase card in _totalCardArr)
        {
            _getCardDic.Add(card.CardInfo.CardName, card);
        }
    }

    public CardBase GetCard(string cardName)
    {
        return _getCardDic[cardName];
    }

    public List<string> GetDeckData(List<CardBase> deck)
    {
        List<string> data = new List<string>();

        foreach(CardBase card in deck)
        {
            data.Add(card.CardInfo.CardName);
        }

        return data;
    }

    public List<CardBase> GetDeck(List<string> deckData)
    {
        List<CardBase> _deck = new ();

        foreach(string cardName in deckData)
        {
            _deck.Add(_getCardDic[cardName]);
        }

        return _deck;
    }
}
