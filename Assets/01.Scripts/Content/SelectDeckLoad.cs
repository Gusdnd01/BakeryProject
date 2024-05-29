using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectDeckLoad : MonoBehaviour
{
    private void Start()
    {
        if(DataManager.Instance.IsHaveData(DataKeyList.playerDeckDataKey))
        {
            List<string> deckData =
            DataManager.Instance.LoadData<PlayerSelectDeckInfoData>(DataKeyList.playerDeckDataKey).
            PlayerSelectDeck;

            MapManager.Instanace.SelectDeck = DeckManager.Instance.GetDeck(deckData);
        }
    }
}
