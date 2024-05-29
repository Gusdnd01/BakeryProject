using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DeckSearch : MonoBehaviour
{
    [SerializeField] private DeckGenerator _deckGenerator;
    private TMP_InputField _deckNameInput;

    private void Awake()
    {
        _deckNameInput = GetComponent<TMP_InputField>();
        _deckNameInput.onEndEdit.AddListener(HandleSearchDeck);
    }

    private void HandleSearchDeck(string deckName)
    {
        List<DeckElement> filteringList = new List<DeckElement>();

        foreach (DeckElement de in _deckGenerator.CurrentDeckList)
        {
            if (de.deckName.Contains(deckName))
            {
                filteringList.Add(de);
            }
        }

        _deckGenerator.FilteringDeckList(filteringList);
        //if (deckName != string.Empty)
        //{
            

        //}
        //else
        //{
        //    _deckGenerator.ResetDeckList(_deckGenerator.CurrentDeckList);
        //}
    }
}
