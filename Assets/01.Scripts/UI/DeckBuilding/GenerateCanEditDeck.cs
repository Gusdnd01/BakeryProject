using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateCanEditDeck : DeckGenerator
{
    [SerializeField] private EditDeckPanel _editDeckPanel;

    protected override void Start()
    {
        base.Start();
    }

    protected override void SetSelectDeck(DeckElement deckElement)
    {
        _editDeckPanel.gameObject.SetActive(true);
        _editDeckPanel.SetPanelInfo(deckElement);
    }
}
