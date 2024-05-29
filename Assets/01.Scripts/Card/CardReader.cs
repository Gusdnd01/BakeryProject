using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public static class CardReader
{
    public static List<CardBase> _inHandCardList = new List<CardBase>();
    private static List<CardBase> _inDeckCardList = new List<CardBase>();
    public static List<CardBase> captureHandList = new List<CardBase>();

    private static CardDrawer _cardDrawer;
    public static CardDrawer CardDrawer
    {
        get
        {
            if(_cardDrawer != null) return _cardDrawer;

            _cardDrawer = GameObject.FindObjectOfType<CardDrawer>();
            return _cardDrawer;
        }
    }

    private static CombineMaster _combineMaster;
    public static CombineMaster CombineMaster
    {
        get
        {
            if (_combineMaster != null) return _combineMaster;

            _combineMaster = GameObject.FindObjectOfType<CombineMaster>();
            return _combineMaster;
        }
    }

    private static SkillCardManagement _skillCardManagement;
    public static SkillCardManagement SkillCardManagement
    {
        get
        {
            if(_skillCardManagement != null) return _skillCardManagement;
            _skillCardManagement = GameObject.FindObjectOfType<SkillCardManagement>();
            return _skillCardManagement;
        }
    }

    private static SpellCardManagement _spellCardManagement;
    public static SpellCardManagement SpellCardManagement
    {
        get
        {
            if(_spellCardManagement != null) return _spellCardManagement;
            _spellCardManagement = GameObject.FindObjectOfType<SpellCardManagement>();
            return _spellCardManagement;
        }
    }

    private static InGameError _inGameError;
    public static InGameError InGameError
    {
        get
        {
            if(_inGameError != null) return _inGameError;
            _inGameError = GameObject.FindObjectOfType<InGameError>();
            return _inGameError;
        }
    }

    private static AbilityTargettingSystem _abilityTargetSystem;
    public static AbilityTargettingSystem AbilityTargetSystem
    {
        get
        {
            if (_abilityTargetSystem != null) return _abilityTargetSystem;
            _abilityTargetSystem = GameObject.FindObjectOfType<AbilityTargettingSystem>();
            return _abilityTargetSystem;
        }
    }

    private static CardProductionMaster _cardProductionMaster;
    public static CardProductionMaster CardProductionMaster
    {
        get
        {
            if(_cardProductionMaster != null) return _cardProductionMaster;
            _cardProductionMaster = GameObject.FindObjectOfType<CardProductionMaster>();
            return _cardProductionMaster;
        }
    }

    public static CardBase OnPointerCard { get; set; }
    public static bool OnBinding { get; set; }

    public static CardBase ShufflingCard { get; set; }
    private static int _deckIdx;

    public static void CaptureHand()
    {
        captureHandList.Clear();

        foreach(CardBase cb in _inHandCardList)
        {
            captureHandList.Add(cb);
        }
    }

    public static bool IsSameCaptureHand()
    {
        if(captureHandList.Count != _inHandCardList.Count) return false;

        for(int i = 0; i < captureHandList.Count; i++)
        {
            if (captureHandList[i].CardInfo != _inHandCardList[i].CardInfo) return false;
        }
        return true;
    }

    public static void SetDeck(List<CardBase> deck)
    {
        _deckIdx = 0;
        _inHandCardList.Clear();
        _inDeckCardList = deck;
    }

    public static void ResetByCaptureHand()
    {
        _inHandCardList.Clear();

        foreach(CardBase cb in captureHandList)
        {
            _inHandCardList.Add(cb);
        }
    }

    public static List<CardBase> GetHandCards()
    {
        return _inHandCardList;
    }

    public static void AddCardInHand(CardBase addingCardInfo)
    {
        _inHandCardList.Add(addingCardInfo);
    }

    public static void RemoveCardInHand(CardBase removingCardInfo)
    {
        _inHandCardList.Remove(removingCardInfo);
    }

    public static int CountOfCardInHand()
    {
        return _inHandCardList.Count;
    }

    public static CardBase GetCardinfoInHand(int index)
    {
        if(index < 0 || index > CountOfCardInHand()) return null;

        return _inHandCardList[index];
    }

    public static void AddCardInDeck(CardBase addingCardInfo)
    {
        _inDeckCardList.Add(addingCardInfo);
    }

    public static void RemoveCardInDeck(CardBase removingCardInfo)
    {
        _inDeckCardList.Remove(removingCardInfo);
    }

    public static int CountOfCardInDeck()
    {
        return _inDeckCardList.Count;
    }

    
    public static CardBase GetCardInDeck()
    {
        _deckIdx %= _inDeckCardList.Count;
        return _inDeckCardList[_deckIdx++];
    }

    public static CardBase GetRandomCardInDeck()
    {
        return _inDeckCardList[Random.Range(0, _inDeckCardList.Count)];
    }

    public static int GetIdx(CardBase handCard)
    {
        return _inHandCardList.IndexOf(handCard);
    }

    public static void ShuffleInHandCard(CardBase pointerCard, CardBase shufflingCard)
    {
        ShufflingCard = shufflingCard;

        int idx1 = _inHandCardList.IndexOf(pointerCard);
        int idx2 = _inHandCardList.IndexOf(shufflingCard);

        (_inHandCardList[idx1], _inHandCardList[idx2]) =
        (_inHandCardList[idx2], _inHandCardList[idx1]);
    }

    public static int GetPosOnTopDrawCard()
    {
        return 860 - ((CountOfCardInHand() -1) * 170);
    }

    public static int GetHandPos(CardBase cardBase)
    {
        int idx = _inHandCardList.IndexOf(cardBase);
        return 860 - (idx * 170);
    }

    public static void LockHandCard(bool isLock)
    {
        foreach(CardBase card in _inHandCardList)
        {
            card.CanUseThisCard = !isLock;
        }
    }
}
