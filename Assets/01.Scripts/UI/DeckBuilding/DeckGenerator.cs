using DG.Tweening;
using ExtensionFunction;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeckGenerator : MonoBehaviour
{
    [SerializeField] private RectTransform _deckElemetTrm;
    [SerializeField] private GridLayoutGroup _gridLayoutGroup;
    [SerializeField] private CanUseDeckElement _canUseDeckPrefab;
    [SerializeField] private SelectedDeck _selectDeckObj;
    [SerializeField] private int _startPage = 1;
    [SerializeField] private TextMeshProUGUI _pageText;
    private int _currentPage;

    private DeckElement _selectDeck;
    public DeckElement SelectDeck
    {
        get
        {
            return _selectDeck;
        }
        set
        {
            _selectDeck = value;
            SetSelectDeck(value);
        }
    }

    private SaveDeckData _saveDeckData = new SaveDeckData();
    private Tween _scalingTween;

    public List<DeckElement> CurrentDeckList { get; private set; } = new List<DeckElement>();

    protected virtual void Start()
    {
        _currentPage = _startPage;

        GenerateDeckList();
        ResetDeckList();
    }

    public void GenerateDeckList()
    {
        if (DataManager.Instance.IsHaveData(DataKeyList.saveDeckDataKey))
        {
            _saveDeckData = DataManager.Instance.LoadData<SaveDeckData>(DataKeyList.saveDeckDataKey);
        }

        foreach(DeckElement de in _saveDeckData.SaveDeckList)
        {
            CurrentDeckList.Add(de);
        }
    }

    private void SetPageText()
    {
        _pageText.text = $"{_currentPage} ÆäÀÌÁö";
    }

    public void GoAfterPage()
    {
        if(_currentPage > Mathf.CeilToInt(_saveDeckData.SaveDeckList.Count / 4))
        {
            Debug.Log(_saveDeckData.SaveDeckList.Count);
            return;
        }

        ++_currentPage;
        SetPageText();
        ResetDeckList();
    }

    public void GoBeforePage()
    {
        if (_currentPage - 1 <= 0)
        {
            return;
        }

        --_currentPage;
        SetPageText();
        ResetDeckList();
    }

    public void ResetDeckList()
    {
        GenerateDeckList(_saveDeckData.SaveDeckList);
    }

    public void FilteringDeckList(List<DeckElement> deList)
    {
        GenerateDeckList(deList);
    }

    private void GenerateDeckList(List<DeckElement> deList)
    {
        _deckElemetTrm.Clear();

        int startIdx = 4 * (_currentPage - 1);
        int maxIdx;

        if (_currentPage * 4 <= deList.Count)
        {
            maxIdx = 4;
        }
        else
        {
            maxIdx = startIdx + (deList.Count % 4);
        }

        if (startIdx == maxIdx) return;

        string deckName = string.Empty;
        if (DataManager.Instance.IsHaveData(DataKeyList.playerDeckDataKey))
        {
            deckName =
            DataManager.Instance.LoadData<PlayerSelectDeckInfoData>(DataKeyList.playerDeckDataKey).deckName;
        }

        for (int i = startIdx; i < maxIdx; i++)
        {
            CanUseDeckElement cude = Instantiate(_canUseDeckPrefab, _deckElemetTrm);
            Debug.Log($"{deList[i].deckName}, {deckName}");
            cude.SetDeckInfo(deList[i], this, deList[i].deckName == deckName);
        }

        _scalingTween?.Kill();
        _scalingTween = DOTween.To(() => new Vector2(320, 320),
                        x => _gridLayoutGroup.cellSize = x, new Vector2(330, 330), 0.2f).
       OnComplete(() => DOTween.To(() => new Vector2(330, 330),
                        x => _gridLayoutGroup.cellSize = x, new Vector2(320, 320), 0.2f));
    }

    protected virtual void SetSelectDeck(DeckElement deckElement)
    {
        if(deckElement.deck == null)
        {
            _selectDeckObj.gameObject.SetActive(false);
            return;
        }
        print(deckElement.deck);
        _selectDeckObj.gameObject.SetActive(true);
        _selectDeckObj.SetDeckInfo(deckElement.deckName, 
                                   DeckManager.Instance.GetDeck(deckElement.deck));
    }
}
