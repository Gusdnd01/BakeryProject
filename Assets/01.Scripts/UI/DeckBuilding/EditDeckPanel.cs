using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class EditDeckPanel : MonoBehaviour
{
    private DeckElement _editDeckElement;

    [Header("ÂüÁ¶")]
    [SerializeField] private TextMeshProUGUI _deckNameText;
    [SerializeField] private Image[] _inDeckCardArr = new Image[5];

    [SerializeField] private UnityEvent<DeckElement> _deckEditEvent;
    [SerializeField] private UnityEvent _deckRemoveEvent;
    [SerializeField] private UnityEvent<List<DeckElement>> _reloadEvent;

    private Vector2 _randomRiseValue = new Vector2(1, 2);
    private Sequence _toRiseCardSeq = null;

    private SaveDeckData _saveDeckData = new SaveDeckData();

    public void SetPanelInfo(DeckElement deckElement)
    {
        _editDeckElement = deckElement;

        _deckNameText.text = deckElement.deckName;
        List<CardBase> _deck = DeckManager.Instance.GetDeck(deckElement.deck);

        for(int i = 0; i < _deck.Count; i++)
        {
            _inDeckCardArr[i].sprite = _deck[i].CardInfo.CardVisual;
            TextMeshProUGUI cost =_inDeckCardArr[i].transform.Find("CsotText").GetComponent<TextMeshProUGUI>();
            cost.text = _deck[i].AbilityCost.ToString();
        }
    }

    public void RemoveDeck()
    {
        if(DataManager.Instance.IsHaveData(DataKeyList.saveDeckDataKey))
        {
            _saveDeckData = DataManager.Instance.LoadData<SaveDeckData>(DataKeyList.saveDeckDataKey);
        }

        DeckElement de = _saveDeckData.SaveDeckList.Find(x => x.deckName == _editDeckElement.deckName);
        _saveDeckData.SaveDeckList.Remove(de);
        DataManager.Instance.SaveData(_saveDeckData, DataKeyList.saveDeckDataKey);
        _deckRemoveEvent?.Invoke();
        _reloadEvent?.Invoke(_saveDeckData.SaveDeckList);

        gameObject.SetActive(false);
    }

    public void EditDeck()
    {
        if (DataManager.Instance.IsHaveData(DataKeyList.saveDeckDataKey))
        {
            _saveDeckData = DataManager.Instance.LoadData<SaveDeckData>(DataKeyList.saveDeckDataKey);
        }

        DeckElement de = _saveDeckData.SaveDeckList.Find(x => x.deckName == _editDeckElement.deckName);
        int idx = _saveDeckData.SaveDeckList.IndexOf(de);
        DeckManager.Instance.SaveDummyDeck = (de, idx);
        _saveDeckData.SaveDeckList.Remove(de);

        DataManager.Instance.SaveData(_saveDeckData, DataKeyList.saveDeckDataKey);
        _deckEditEvent?.Invoke(_editDeckElement);
    }

    private void OnEnable()
    {
        _toRiseCardSeq = DOTween.Sequence();

        foreach(Image cardImg in _inDeckCardArr)
        {
            float currentY = cardImg.transform.localPosition.y;
            float randomValue = Random.Range(_randomRiseValue.x, _randomRiseValue.y);

            _toRiseCardSeq.Join(cardImg.transform.DOLocalMoveY(currentY + 10, randomValue)
                          .SetLoops(-1, LoopType.Yoyo));
        }
    }
    private void OnDisable()
    {
        _toRiseCardSeq?.Kill();
    }
}
