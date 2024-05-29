using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckBuildingUI : SceneUI
{
    private DeckBuilder _deckBuilder;

    public override void SceneUIStart()
    {
        _deckBuilder = GetComponent<DeckBuilder>();
    }

    public void OnDestroy()
    {
        DataGenerate();
    }

    public void DataGenerate()
    {
        SaveDeckData saveDeckData = DataManager.Instance.LoadData<SaveDeckData>(DataKeyList.saveDeckDataKey);

        if (!_deckBuilder.IsDeckSaving)
        {
            saveDeckData.SaveDeckList.Insert(DeckManager.Instance.SaveDummyDeck.Item2, DeckManager.Instance.SaveDummyDeck.Item1);
        }

        DataManager.Instance.SaveData(saveDeckData, DataKeyList.saveDeckDataKey);
    }
}
