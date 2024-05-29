using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DeckBuilder : MonoBehaviour
{
    private SaveDeckData _saveDeckData = new SaveDeckData();

    [SerializeField] private GameObject _doNotSaving;
    public ExpansionList<CardBase> selectCardList = new ExpansionList<CardBase>();
    [SerializeField] private SelectCard[] _selectCardElementArr = new SelectCard[5];

    private bool _isDeckSaving;
    public bool IsDeckSaving
    {
        get
        {
            return _isDeckSaving;
        }
        set
        {
            _isDeckSaving = value;
            _doNotSaving.SetActive(!_isDeckSaving);
        }
    }

    [SerializeField] private UnityEvent<string> _errorEvent = null;

    private void Start()
    {
        if(DataManager.Instance.IsHaveData(DataKeyList.saveDeckDataKey))
        {
            _saveDeckData = DataManager.Instance.LoadData<SaveDeckData>(DataKeyList.saveDeckDataKey);
        }
    }

    public void AddDeck(CardBase cardBase, out bool canSelect)
    {
        if(selectCardList.Count >= 5)
        {
            _errorEvent?.Invoke(ErrorTextBase.deckCountError);
            canSelect = false;
            return;
        }

        for(int i = 0; i <  selectCardList.Count; i++)
        {
            if (selectCardList[i].CardInfo.CardName == cardBase.CardInfo.CardName)
            {
                _errorEvent?.Invoke(ErrorTextBase.cardTypeError);
                canSelect = false;
                return;
            }
        }

        canSelect = true;
        selectCardList.Add(cardBase);
        foreach(SelectCard sc in _selectCardElementArr)
        {
            if(!sc.IsAssignedCard)
            {
                sc.SetCard(cardBase.CardInfo, (int)cardBase.CombineLevel);
                break;
            }
        }
    }

    public void RemoveInDeck(CardBase cardBase)
    {
        foreach (SelectCard sc in _selectCardElementArr)
        {
            if (sc.SelectCardInfo == cardBase.CardInfo)
            {
                sc.UnSetCard();
                selectCardList.Remove(cardBase);
                break;
            }
        }
    }

    public void SaveDeck(string deckName)
    {
        if (selectCardList.Count < 5)
        {
            _errorEvent?.Invoke(ErrorTextBase.plzAllSelectError);
            return;
        }

        if (deckName.Length > 20 || deckName.Length == 0)
        {
            _errorEvent?.Invoke(ErrorTextBase.deckNameError);
            return;
        }

        IsDeckSaving = true;

        List<string> convertDataDeck = new List<string>();
        foreach (CardBase card in selectCardList)
        {
            convertDataDeck.Add(card.CardInfo.CardName);
        }

        DeckElement de = new DeckElement(deckName, convertDataDeck);
        _saveDeckData.SaveDeckList.Add(de);
        DataManager.Instance.SaveData(_saveDeckData, DataKeyList.saveDeckDataKey);
    }
}
