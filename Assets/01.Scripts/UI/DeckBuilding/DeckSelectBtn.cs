using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckSelectBtn : MonoBehaviour
{
    private PlayerSelectDeckInfoData _deckInfoData = new PlayerSelectDeckInfoData();
    [SerializeField] private SelectedDeck _deck;

    public void DeckSelect()
    {
        _deckInfoData.PlayerSelectDeck = DeckManager.Instance.GetDeckData(_deck.SelectDeck);
        DataManager.Instance.SaveData(_deckInfoData, DataKeyList.playerDeckDataKey);
        MapManager.Instanace.SelectDeck = _deck.SelectDeck;
    }
}
