using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct DeckElement
{
    public string deckName;
    public List<string> deck;

    public DeckElement(string _deckName, List<string> _deck)
    {
        deckName = _deckName;
        deck = _deck;
    }
}

public class SaveDeckData : CanSaveData
{
    public List<DeckElement> SaveDeckList = new List<DeckElement>();

    public override void SetInitialValue()
    {

    }
}
