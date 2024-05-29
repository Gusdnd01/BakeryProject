using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CardShameContainer : MonoBehaviour
{
    [SerializeField] private List<CardInfo> _cardShameDataContainer;
    [SerializeField] private UnityEvent<List<CardInfo>> _cardElementGeneratingEvent;

    private void Start()
    {
        _cardElementGeneratingEvent?.Invoke(_cardShameDataContainer);
    }

    public CardShameElementSO GetCardShameData(CardInfo info)
    {
        return _cardShameDataContainer.Find(x => x == info).cardShameData;
    }
}
