using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSelecter : MonoBehaviour
{
    [SerializeField] private CardSelectElement _cardSelectPrefab;
    [SerializeField] private DeckBuilder _deckBuilder;
    [SerializeField] private RectTransform _hasCardListTrm;

    private CanUseCardData _canUseCardData = new CanUseCardData();
    private const string _canUseCardDataKey = "CanUseCardsDataKey";

    private List<CardSelectElement> _cardArr = new List<CardSelectElement>();  

    public void AutoSelectCard(CardBase cardBase)
    {
        if(_cardArr.Count < 0)
        {
            Debug.LogError("하야스기르");
            return;
        }

        foreach(CardSelectElement element in _cardArr)
        {
            if(element.CardBase.CardInfo == cardBase.CardInfo) 
            {
                element.OnPointerClick(null);
                return;
            }
        }
    }

    private void Start()
    {
        if (DataManager.Instance.IsHaveData(_canUseCardDataKey))
        {
            _canUseCardData = DataManager.Instance.LoadData<CanUseCardData>(_canUseCardDataKey);
        }

        for(int i = 0; i < _canUseCardData.CanUseCardsList.Count; i++)
        {
            if(i % 6 == 0)
            {
                _hasCardListTrm.sizeDelta += new Vector2(0, 460);
            }

            CardSelectElement cse = Instantiate(_cardSelectPrefab, _hasCardListTrm);
            _cardArr.Add(cse);
            cse.SetInfo(DeckManager.Instance.GetCard(_canUseCardData.CanUseCardsList[i]), _deckBuilder);
        }
    }
}
