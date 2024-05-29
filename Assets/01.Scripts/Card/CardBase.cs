using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using CardDefine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System;

public abstract class CardBase : MonoBehaviour, IPointerClickHandler, 
                                 IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private float _toMovePosInSec;
    public RectTransform VisualRectTrm { get; private set; }
    public CardInfo CardInfo => _myCardInfo;
    [SerializeField] private CardInfo _myCardInfo;
    public bool CanUseThisCard { get; set; } = false;
    public bool IsOnActivationZone { get; set; }
    [SerializeField] private GameObject[] _objArr = new GameObject[3];
    [SerializeField] private CombineLevel _combineLevel;
    public CombineLevel CombineLevel
    {
        get
        {
            return _combineLevel;
        }
        set
        {
            _combineLevel = value;
            _objArr[(int)_combineLevel].SetActive(true);
        }
    }
    [SerializeField] private Transform visualTrm;
    public Transform VisualTrm
    {
        get
        {
            return visualTrm;
        }
        
    }
    private bool _isActivingAbillity;
    protected bool IsActivingAbillity
    {
        get
        {
            return _isActivingAbillity;
        }
        set
        {
            _isActivingAbillity = value;

            if(_isActivingAbillity)
            {
                CardReader.LockHandCard(true);
            }
            else
            {
                ExitThisCard();
            }
        }
    }
    [SerializeField] private Material _cardMat;
    public int AbilityCost => CardManagingHelper.GetCardShame(CardInfo.cardShameData,
                                                                  CardShameType.Cost,
                                                                  (int)CombineLevel);

    [HideInInspector]public BattleController battleController;
    protected Player Player => battleController.Player;

    [SerializeField]protected BuffSO buffSO;
    [SerializeField] protected SEList<SEList<int>> damageArr;

    private TextMeshProUGUI _costText;

    protected List<Entity> targets = new();

    protected Color minimumColor = new Color(255, 255, 255, .1f);
    protected Color maxtimumColor = new Color(255, 255, 255, 1.0f);

    public Action<Transform> OnPointerSetCardAction { get; set; }
    public Action<Transform> OnPointerInitCardAction { get; set; }

    public float CardIdlingAddValue { get; set; }
    public bool OnPointerInCard { get; set; }   

    private void Awake()
    {
        VisualRectTrm = VisualTrm.GetComponent<RectTransform>();
        _costText = transform.Find("Visual/CsotText").GetComponent<TextMeshProUGUI>();

        _costText.text = AbilityCost.ToString();
    }

    private void OnDestroy()
    {
        OnPointerSetCardAction = null;
        CardReader.CardProductionMaster.QuitCardling(this);
    }

    public abstract void Abillity();

    public void ActiveInfo()
    {
        CardReader.SkillCardManagement.SetCardInfo(CardInfo, true);
        VisualRectTrm.DOScale(1.3f, 0.2f);

        Vector2 pos = transform.localPosition;
        transform.DOLocalMove(new Vector2(pos.x - 50, pos.y + 40), 0.3f);
    }
    private void ExitThisCard()
    {
        Image img = VisualRectTrm.GetComponent<Image>();
        Material mat = new Material(_cardMat);
        img.material = mat;

        Sequence seq = DOTween.Sequence();
        seq.Append(DOTween.To(() => mat.GetFloat("_dissolve_amount"), d => mat.SetFloat("_dissolve_amount", d), -0.1f, 2f));
        seq.InsertCallback(1, () =>
        {
            CardReader.SkillCardManagement.SetCardInfo(CardInfo, false);
            CardReader.SkillCardManagement.ChainingSkill();
            CardReader.LockHandCard(false);

            Destroy(gameObject);
        });
    }
    public void SetUpCard(float moveToXPos, bool generateCallback)
    {
        CanUseThisCard = false;
        Vector2 movePos = new Vector2(moveToXPos, -60);

        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOLocalMove(movePos, _toMovePosInSec).SetEase(Ease.OutBack));
        seq.Join(transform.DOLocalRotateQuaternion(Quaternion.identity, _toMovePosInSec).SetEase(Ease.OutBack));
        seq.AppendCallback(() =>
        {
            if(generateCallback)
            {
                CardReader.CombineMaster.CombineGenerate();
                CardReader.OnPointerCard = null;
            }
            CanUseThisCard = true;
        });
    }
    public bool CheckCanCombine(out CardBase frontCard)
    {
        if (CardReader.GetIdx(this) != 0)
        {
            CardBase frontOfThisCard = CardReader.GetCardinfoInHand(CardReader.GetIdx(this) - 1);
            if (frontOfThisCard.CardInfo.CardName == _myCardInfo.CardName &&
                frontOfThisCard.CombineLevel == CombineLevel &&
                frontOfThisCard.CombineLevel != CombineLevel.III)
            {
                frontCard = frontOfThisCard;
                return true;
            }
            else
            {
                frontCard = null;
                return false;
            }
        }
        else
        {
            frontCard = null;
            return false;
        }
    }
    private void Shuffling()
    {
        CardReader.ShuffleInHandCard(CardReader.OnPointerCard, this);
        SetUpCard(CardReader.GetHandPos(this), false);
    }
    private void Update()
    {
        if (!CanUseThisCard) return;

        if (CardReader.OnPointerCard == null ||
            CardReader.OnPointerCard == this ||
            CardReader.OnBinding   == false) return;

        if (UIFunction.IsImagesOverlapping(CardReader.OnPointerCard.VisualRectTrm, VisualRectTrm))
        {
            if(CardReader.OnPointerCard.transform.position.x > transform.position.x
            && CardReader.GetIdx(CardReader.OnPointerCard) > CardReader.GetIdx(this))
            {
                Shuffling();
            }
            else if(CardReader.OnPointerCard.transform.position.x < transform.position.x
                 && CardReader.GetIdx(CardReader.OnPointerCard) < CardReader.GetIdx(this))
            {
                Shuffling();
            }
        }
    }
    public int[] GetDamage(CombineLevel level)
    {
        CardManagingHelper.GetCardShame(CardInfo.cardShameData, CardShameType.Damage,(int)level);
        return damageArr.list[(int)level].list.ToArray();
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!IsOnActivationZone) return;

        CardReader.AbilityTargetSystem.ActivationCardSelect(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (IsOnActivationZone) return;

        OnPointerSetCardAction?.Invoke(transform);
        OnPointerInCard = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (IsOnActivationZone) return;

        OnPointerInitCardAction?.Invoke(transform);
        OnPointerInCard = false;
    }
}
