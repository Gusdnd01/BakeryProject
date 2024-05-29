using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SkillCardManagement : CardManagement
{
    [SerializeField] private TargettingMaskCreater _maskCreater;
    private ExpansionList<CardBase> InCardZoneCatalogue = new ExpansionList<CardBase>();
    public List<CardBase> InCardZoneList => InCardZoneCatalogue;

    [Header("대기존 셋팅값")]
    [SerializeField] private Transform _cardWaitZone;
    [SerializeField] private Vector2 _normalZonePos;
    [SerializeField] private float _waitTurmValue = 85f;
    [SerializeField] private Transform _cardInfoTrm;
    private CardInfoPanel _cardInfoPanel;

    [Header("발동존 셋팅값")]
    [SerializeField] private Transform _activationCardZone;
    [SerializeField] private Vector2 _lastCardPos;
    [SerializeField] private float _activationTurmValue = 155f;

    [Header("이벤트")]
    private bool _isInChaining;
    public UnityEvent useCardEndEvnet;
    public UnityEvent beforeChainingEvent;
    [SerializeField] private UnityEvent _afterChanningEvent;
    [SerializeField] private UnityEvent<bool> _acceptBtnSwitchEvent;
    [SerializeField] private UnityEvent _checkStageClearEvent;
    [SerializeField] private UnityEvent<bool> _setupHandCardEvent;

    private void Start()
    {
        InCardZoneCatalogue.ListChanged += HandleCheckAcceptBtn;
    }
    private void HandleCheckAcceptBtn(object sender, EventArgs e)
    {
        _acceptBtnSwitchEvent?.Invoke(InCardZoneCatalogue.Count != 0);
    }
    public void SetupCardsInActivationZone()
    {
        CardReader.AbilityTargetSystem.ChainFadeControl(0);
        CardReader.AbilityTargetSystem.FadingAllChainTarget(0);

        _setupHandCardEvent?.Invoke(false);
        _acceptBtnSwitchEvent?.Invoke(false);
        int maxCount = InCardZoneCatalogue.Count;

        for (int i = 0; i < maxCount; i++)
        {
            float x = _lastCardPos.x - (_activationTurmValue * (maxCount - i - 1));
            Vector2 targetPos = new Vector2(x, _lastCardPos.y);
            Transform selectTrm = InCardZoneCatalogue[i].transform;

            selectTrm.SetParent(_activationCardZone);

            Sequence seq = DOTween.Sequence();
            seq.Append(selectTrm.DOLocalRotate(new Vector3(0, 0, 10), 0.1f));
            seq.Append(selectTrm.DOLocalMove(targetPos, 0.5f).SetEase(Ease.InOutBack));
            seq.Join(selectTrm.DOLocalRotate(Vector3.zero, 0.5f));

            if (i == maxCount - 1)
            {
                seq.InsertCallback(1, () => 
                { 
                    ChainingSkill();
                });
            }
        }
    }

    public void ChainingSkill()
    {
        if (_isInChaining)
            useCardEndEvnet?.Invoke();

        if (!_isInChaining && InCardZoneCatalogue.Count != 0)
        {
            beforeChainingEvent?.Invoke();
            _isInChaining = true;
        }
        else if (_isInChaining && InCardZoneCatalogue.Count == 0)
        {
            _afterChanningEvent?.Invoke();
            _isInChaining = false;

            foreach(Transform t in _activationCardZone)
            {
                Destroy(t.gameObject);
            }

            TurnCounter.TurnCounting.ToEnemyTurnChanging(true);
            _setupHandCardEvent?.Invoke(true);
            _checkStageClearEvent?.Invoke();

            CardReader.AbilityTargetSystem.AllChainClear();
            
            return;
        }
        
        CardBase selectCard = InCardZoneCatalogue[0];
        InCardZoneCatalogue.Remove(selectCard);

        selectCard.ActiveInfo();
        selectCard.battleController.BattleCutSlider.Reverting();
        UseAbility(selectCard);
    }

    public override void UseAbility(CardBase selectCard)
    {
        selectCard.battleController.CameraController.
        StartCameraSequnce(selectCard.CardInfo.cameraSequenceData);

        selectCard.battleController.BattleCutSlider.Cutting();

        selectCard.Abillity();
    }

    public void SetSkillCardInCardZone(CardBase selectCard)
    {
        selectCard.CanUseThisCard = false;

        selectCard.transform.SetParent(_cardWaitZone);
        CardReader.RemoveCardInHand(CardReader.OnPointerCard);
        InCardZoneCatalogue.Add(selectCard);
        selectCard.IsOnActivationZone = true;

        selectCard.transform.DOScale(1.1f, 0.3f);
        
        GenerateCardPosition(selectCard);
        CardReader.CombineMaster.CombineGenerate();
        CardReader.CaptureHand();
    }
    private void GenerateCardPosition(CardBase selectCard)
    {
        CardReader.AbilityTargetSystem.AllGenerateChainPos(true);
        Sequence seq = DOTween.Sequence();

        int maxIdx = InCardZoneCatalogue.Count - 1;

        if (maxIdx != 0)
        {
            seq.Append(selectCard.transform.
            DOLocalMove(new Vector2(InCardZoneCatalogue[maxIdx - 1].transform.localPosition.x
                                    + 100, 150), 0.3f));
        }
        else
        {
            seq.Append(selectCard.transform.DOLocalMove(new Vector3(0, 150, 0), 0.3f));
        }

        seq.Join(selectCard.transform.DOLocalRotateQuaternion(Quaternion.identity, 0.3f));

        for (int i = 0; i < maxIdx; i++)
        {
            Transform selectTrm = InCardZoneCatalogue[i].transform;
            seq.Join(selectTrm.DOLocalMove(new Vector2(selectTrm.localPosition.x - 100f, 150), 0.3f));
        }
        seq.AppendCallback(() => 
        {
            CardReader.AbilityTargetSystem.ActivationCardSelect(CardReader.OnPointerCard);
            CardReader.AbilityTargetSystem.SetMouseAndCardArrowBind(CardReader.OnPointerCard);
            CardReader.AbilityTargetSystem.AllGenerateChainPos(false);
        });
    }

    public void SetCardInfo(CardInfo info, bool isSet)
    {
        if (isSet)
        {
            _cardInfoPanel = PoolManager.Instance.Pop(PoolingType.CardInfoPanel) as CardInfoPanel;
            _cardInfoPanel.SetInfo(info, _cardInfoTrm);
        }
        else
        {
            _cardInfoPanel.UnSetInfo();
        }

    }
}
