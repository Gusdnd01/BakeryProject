using DG.Tweening;
using ExtensionFunction;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CanUseDeckElement : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public DeckElement DeckInfo { get; private set; }

    [SerializeField] private Image[] _cardGroupArr = new Image[5];
    [SerializeField] private TextMeshProUGUI _deckNameText;
    [SerializeField] private float _onPointerEnterMoveY; 
    [SerializeField] private float _onPointerExitMoveY; 
    private DeckElement _deckInfo;

    [SerializeField] private CanvasGroup _deckSelectBubble;
    [SerializeField] private CanvasGroup _deckEditBubble;
    [SerializeField] private GameObject _selectMarkObj;
    private float _selectBubbleY;
    private float _editBubbleY;

    private DeckGenerator _deckGenerator;
    private string _deckName;

    private PlayerSelectDeckInfoData _deckInfoData = new PlayerSelectDeckInfoData();

    private void SetBubbleInteractive(bool canInteractive)
    {
        _deckSelectBubble.interactable = canInteractive;
        _deckEditBubble.interactable = canInteractive;
    }

    public void HandleDeckEdit()
    {
        SetBubbleInteractive(false);
        _deckGenerator.SelectDeck = _deckInfo;
    }

    public void HandleDeckSelect()
    {
        SetBubbleInteractive(false);

        _deckInfoData.deckName = DeckInfo.deckName;
        _deckInfoData.PlayerSelectDeck = DeckInfo.deck;
        DataManager.Instance.SaveData(_deckInfoData, DataKeyList.playerDeckDataKey);

        _deckGenerator.ResetDeckList();
    }

    public void SetDeckInfo(DeckElement deckInfo, DeckGenerator deckGenerator, bool isThisDeckSelected)
    {
        _selectMarkObj.SetActive(isThisDeckSelected);

        DeckInfo = deckInfo;
        _deckGenerator = deckGenerator;

        for(int i = 0; i < _cardGroupArr.Length; i++)
        {
            CardBase card = DeckManager.Instance.GetCard(deckInfo.deck[i]);
            _cardGroupArr[i].sprite = card.CardInfo.CardVisual;
            TextMeshProUGUI costText = _cardGroupArr[i].GetComponentInChildren<TextMeshProUGUI>();
            costText.text = card.AbilityCost.ToString();
        }

        _deckName = deckInfo.deckName;

        if(_deckName.Length > 6)
        {
            _deckName = $"{_deckName.Substring(0, 6)}.."; 
        }

        _deckNameText.text = _deckName;
        _deckInfo = deckInfo;

        SetUpBubble();
    }

    private void SetUpBubble()
    {
        _deckSelectBubble.alpha = 0;
        _deckEditBubble.alpha = 0;

        Vector2 localPos;
        ElementBubble elementBubble = _deckEditBubble.GetComponentInChildren<ElementBubble>();
        elementBubble.OnPointerClickEvent += HandleDeckEdit;
        elementBubble.OnPointerClickEvent += HideBubbleGroup;

        elementBubble = _deckSelectBubble.GetComponentInChildren<ElementBubble>();
        elementBubble.OnPointerClickEvent += HandleDeckSelect;
        elementBubble.OnPointerClickEvent += HideBubbleGroup;

        localPos = _deckEditBubble.transform.localPosition;
        _editBubbleY = localPos.y - 10;
        _deckEditBubble.transform.localPosition = new Vector3(localPos.x, _editBubbleY);

        localPos = _deckSelectBubble.transform.localPosition;
        _selectBubbleY = localPos.y - 10;
        _deckSelectBubble.transform.localPosition = new Vector3(localPos.x, _selectBubbleY);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.DOKill();
        transform.DOScale(1.05f, 0.3f);

        for (int i = -2; i <= 2; i++)
        {
            Transform trm = _cardGroupArr[i + 2].transform;

            trm.DOKill();
            trm.DOLocalMove(new Vector2(_cardGroupArr[i + 2].transform.localPosition.x + (i * 5),
                                        _onPointerEnterMoveY + (2 - Mathf.Abs(i)) * 5), 0.3f).SetEase(Ease.OutBack);
            trm.DOLocalRotateQuaternion(Quaternion.Euler(0, 0, -i * 5), 0.3f).SetEase(Ease.OutBack);
        }

        LookBubbleGroup();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.DOKill();
        transform.DOScale(1f, 0.3f);

        for (int i = 0; i < _cardGroupArr.Length; i++)
        {
            float value = (i - 2) * 5;

            Transform trm = _cardGroupArr[i].transform;

            trm.DOKill();
            trm.DOLocalMove(new Vector2(_cardGroupArr[i].transform.localPosition.x - value,
                                        _onPointerExitMoveY), 0.3f).SetEase(Ease.OutBack);
            trm.DOLocalRotateQuaternion(Quaternion.Euler(0, 0, 0), 0.3f).SetEase(Ease.OutBack);
        }

        HideBubbleGroup();
    }

    private void BubbleTweenKill()
    {
        _deckSelectBubble.DOKill();
        _deckSelectBubble.transform.DOKill();
        _deckEditBubble.DOKill();
        _deckEditBubble.transform.DOKill();
    }

    private void LookBubbleGroup()
    {
        BubbleTweenKill();

        _deckSelectBubble.DOFade(1, 0.2f);
        _deckSelectBubble.transform.DOLocalMoveY(_selectBubbleY + 10, 0.2f).SetEase(Ease.OutBack);

        _deckEditBubble.DOFade(1, 0.2f);
        _deckEditBubble.transform.DOLocalMoveY(_editBubbleY + 10, 0.25f).SetEase(Ease.OutBack).OnComplete(() =>
        SetBubbleInteractive(true));
    }

    private void HideBubbleGroup()
    {
        BubbleTweenKill();

        _deckSelectBubble.DOFade(0, 0.2f);
        _deckSelectBubble.transform.DOLocalMoveY(_selectBubbleY, 0.2f).SetEase(Ease.OutBack);

        _deckEditBubble.DOFade(0, 0.2f);
        _deckEditBubble.transform.DOLocalMoveY(_editBubbleY, 0.25f).SetEase(Ease.OutBack);
    }
}
