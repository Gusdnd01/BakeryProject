using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Unity.VisualScripting;
using UnityEngine;

public class AbilityTargettingSystem : MonoBehaviour
{
    private bool _canBinding = true;
    public bool CanBinding
    {
        get => _canBinding;
        set
        {
            _canBinding = value;
            if(_getTargetArrowDic.ContainsKey(_selectCard))
            {
                _getTargetArrowDic[_selectCard][0].ActiveArrow(value);
            }
        }
    }
    [SerializeField] private BattleController _battleController;
    private Dictionary<TargetEnemyCount, Func<CardBase, int, IEnumerator>> _targetCountingActionDic = new();

    [Header("마우스 카드 바인딩")]
    [SerializeField] private AbilityTargetArrow _targetArrowPrefab;
    private Dictionary<CardBase, List<AbilityTargetArrow>> _getTargetArrowDic = new ();
    private CardBase _selectCard;
    public Vector2 mousePos;
    private bool _isBindingMouseAndCard;

    [Header("적 확인")]
    [SerializeField] private LayerMask _whatIsEnemy;
    [SerializeField] private Transform _chainImPact;
    [SerializeField] private Color _reactionColor;
    private List<ChainSelectTarget> _chainTargetList = new();

    public bool OnTargetting { get; private set; }

    public void AllChainClear()
    {
        foreach(Transform chain in transform)
        {
            Destroy(chain.gameObject);
        }

        _getTargetArrowDic.Clear();
        _chainTargetList.Clear();
    }
    public void AllGenerateChainPos(bool isGenerate)
    {
        List<CardBase> onActiveZoneList = CardReader.SkillCardManagement.InCardZoneList;

        foreach (CardBase cb in onActiveZoneList)
        {
            if(_getTargetArrowDic.ContainsKey(cb))
            {
                foreach(AbilityTargetArrow ata in _getTargetArrowDic[cb])
                {
                    ata.IsGenerating = isGenerate;
                }
            }
        }
    }
    public void ActivationCardSelect(CardBase selectCard)
    {
        List<CardBase> onActiveZoneList = CardReader.SkillCardManagement.InCardZoneList;

        foreach (CardBase cb in onActiveZoneList)
        {
            if (_getTargetArrowDic.ContainsKey(cb))
            {
                foreach (AbilityTargetArrow ata in _getTargetArrowDic[cb])
                {
                    if (ata.MarkingEntity.ChainningCardList.Contains(selectCard))
                    {
                        ata.MarkingEntity.SelectChainningCharacter(selectCard.CardInfo.skillPersonalColor, 1);
                        continue;
                    }
                    else
                    {
                        Color unSelectedColor = new Color(0, 0, 0, 0);
                        ata.MarkingEntity.SelectChainningCharacter(unSelectedColor, 0);
                    }
                }
            }
        }
    }
    public void ChainFadeControl(float fadeValue)
    {
        List<CardBase> onActiveZoneList = CardReader.SkillCardManagement.InCardZoneList;

        foreach (CardBase cb in onActiveZoneList)
        {
            if (_getTargetArrowDic.ContainsKey(cb))
            {
                foreach (AbilityTargetArrow ata in _getTargetArrowDic[cb])
                {
                    ata.SetFade(fadeValue);
                }
            }
        }
    }
    public void FadingAllChainTarget(float fadeValue)
    {
        foreach(ChainSelectTarget cst in _chainTargetList)
        {
            cst.SetFade(fadeValue);
        }
    }
    public void SetMouseAndCardArrowBind(CardBase selectCard)
    {
        selectCard.CanUseThisCard = false;
        _battleController.Player.VFXManager.SetBackgroundColor(Color.gray);

        EnemyTargetting(selectCard);
    }

    private void Start()
    {
        foreach(TargetEnemyCount tec in Enum.GetValues(typeof(TargetEnemyCount)))
        {
            if(tec == TargetEnemyCount.ALL)
            {
                _targetCountingActionDic.Add(tec, HandleALLEnemyTargetting);
            }
            else if(tec == TargetEnemyCount.ME)
            {
                _targetCountingActionDic.Add(tec, HandleMeTargetting);
            }
            else
            {
                _targetCountingActionDic.Add(tec, HandleCountEnemyTargetting);
            }
        }
    }

    private IEnumerator HandleMeTargetting(CardBase selectCard, int count)
    {
        _selectCard = selectCard;
        AbilityTargetArrow ata = Instantiate(_targetArrowPrefab, transform);
        ata.ActiveArrow(false);
        if (!_getTargetArrowDic.ContainsKey(selectCard))
        {
            List<AbilityTargetArrow> atlist = new();
            _getTargetArrowDic.Add(selectCard, atlist);
        }
        
        _getTargetArrowDic[selectCard].Add(ata);

        ata.transform.position = selectCard.transform.position;

        Vector2 screenPoint = MaestrOffice.GetScreenPosToWorldPos(_battleController.Player.transform.position);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(UIManager.Instance.CanvasTrm, screenPoint, UIManager.Instance.Canvas.worldCamera, out Vector2 anchoredPosition);

        int idx = _getTargetArrowDic[_selectCard].Count - 1;
        _getTargetArrowDic[_selectCard][idx].ArrowBinding(_selectCard.transform, anchoredPosition);
        _getTargetArrowDic[_selectCard][idx].SetFade(0.5f);

        yield return new WaitForSeconds(0.5f);
        _battleController.Player.VFXManager.SetBackgroundColor(Color.white);
        OnTargetting = false;

        
    }
    private IEnumerator HandleCountEnemyTargetting(CardBase selectCard, int count)
    {
        for (int i = 0; i < count; i++)
        {
            _selectCard = selectCard;
            AbilityTargetArrow ata = Instantiate(_targetArrowPrefab, transform);
            if (!_getTargetArrowDic.ContainsKey(selectCard))
            {
                List<AbilityTargetArrow> atlist = new();
                _getTargetArrowDic.Add(selectCard, atlist);
            }

            _getTargetArrowDic[selectCard].Add(ata);
            ata.transform.position = selectCard.transform.position;
            _isBindingMouseAndCard = true;

            yield return new WaitUntil(() => ata.IsBindSucess);
        }

        foreach (var e in _battleController.onFieldMonsterList)
        {
            _battleController.maskDisableEvent?.Invoke(e);
        }

        _battleController.Player.VFXManager.SetBackgroundColor(Color.white);
        OnTargetting = false;
    }
    private IEnumerator HandleALLEnemyTargetting(CardBase selectCard, int count)
    {
        foreach (Enemy e in _battleController.onFieldMonsterList)
        {
            if (e is null) continue;

            _selectCard = selectCard;
            AbilityTargetArrow ata = Instantiate(_targetArrowPrefab, transform);
            ata.ActiveArrow(false);
            if (!_getTargetArrowDic.ContainsKey(selectCard))
            {
                List<AbilityTargetArrow> atlist = new();
                _getTargetArrowDic.Add(selectCard, atlist);
            }

            _getTargetArrowDic[selectCard].Add(ata);
            ata.transform.position = selectCard.transform.position;

            Vector2 screenPoint = MaestrOffice.GetScreenPosToWorldPos(e.transform.position);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(UIManager.Instance.CanvasTrm, screenPoint, UIManager.Instance.Canvas.worldCamera, out Vector2 anchoredPosition);

            int idx = _getTargetArrowDic[_selectCard].Count - 1;
            _getTargetArrowDic[_selectCard][idx].ArrowBinding(_selectCard.transform, anchoredPosition);
            _getTargetArrowDic[_selectCard][idx].SetFade(0.5f);

            EnemyMarking(e);

            CombatMarkingData data =
                new CombatMarkingData(BuffingType.Targetting,
                $"[{_selectCard.CardInfo.CardName}] 스킬에 \r\n선택되었습니다.", 1);

            e.BuffSetter.AddBuffingMark(data);
        }

        yield return new WaitForSeconds(0.5f);
        foreach (var e in _battleController.onFieldMonsterList)
        {
            _battleController.maskDisableEvent?.Invoke(e);
        }
        _battleController.Player.VFXManager.SetBackgroundColor(Color.white);
        OnTargetting = false;
    }

    private void EnemyTargetting(CardBase selectCard)
    {
        TargetEnemyCount tec = (TargetEnemyCount)CardManagingHelper.GetCardShame(selectCard.CardInfo.cardShameData,
                                                               CardShameType.Range,
                                                               (int)selectCard.CombineLevel);

        StartCoroutine(_targetCountingActionDic[tec].Invoke(selectCard, (int)tec));
        OnTargetting = true;

        foreach (var e in _battleController.onFieldMonsterList)
        {
            _battleController.maskEnableEvent?.Invoke(e);
        }
    }

    private void BindMouseAndCardWithArrow()
    {
        if(CanBinding)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(UIManager.Instance.CanvasTrm,
                                                                Input.mousePosition, Camera.main, out mousePos);
        }

        int idx = _getTargetArrowDic[_selectCard].Count - 1;
        _getTargetArrowDic[_selectCard][idx].ArrowBinding(_selectCard.transform, mousePos);
        _getTargetArrowDic[_selectCard][idx].SetFade(0.5f);
    }
    private void CheckSelectEnemy()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Vector2 pos = MaestrOffice.GetWorldPosToScreenPos(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero, 0, _whatIsEnemy);

            if (hit.transform == null) return;

            if (hit.transform.TryGetComponent<Enemy>(out Enemy e))
            {
                EnemyMarking(e);
                ActivationCardSelect(_selectCard);

                CombatMarkingData data =
                new CombatMarkingData(BuffingType.Targetting,
                $"[{_selectCard.CardInfo.CardName}] 스킬에 \r\n선택되었습니다.", 1);

                e.BuffSetter.AddBuffingMark(data);
            }
        }
    }
    private void EnemyMarking(Enemy e)
    {
        e.ChainningCardList.Add(_selectCard);
        e.SelectedOnAttack(_selectCard);

        int idx = _getTargetArrowDic[_selectCard].Count - 1;
        Tween t = _getTargetArrowDic[_selectCard][idx].ReChainning(() =>
        {
            Instantiate(_chainImPact, e.transform.position, Quaternion.identity);
            DamageTextManager.Instance.PopupReactionText(e.transform.position + new Vector3(0, 1, 0), _reactionColor, "Connect!");
        }, e);

        _isBindingMouseAndCard = false;
    }
    private void Update()
    {
        if(_isBindingMouseAndCard)
        {
            BindMouseAndCardWithArrow();
            CheckSelectEnemy();
        }
    }
}
