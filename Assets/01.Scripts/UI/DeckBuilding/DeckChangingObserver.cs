using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class DeckChangingObserver : MonoBehaviour
{
    [SerializeField] private DeckBuilder _deckBuilder;
    [SerializeField] private TMP_InputField _deckName;

    private void Awake()
    {
        _deckBuilder.selectCardList.ListChanged += HandleDeckChange;
        _deckName.onValueChanged.AddListener(HandleDeckNameChange);
    }

    public void SaveDeck()
    {
        _deckBuilder.SaveDeck(_deckName.text);
    }

    private void HandleDeckNameChange(string name)
    {
        _deckBuilder.IsDeckSaving = false;
    }

    private void HandleDeckChange(object sender, EventArgs e)
    {
        _deckBuilder.IsDeckSaving = false;
    }
}
