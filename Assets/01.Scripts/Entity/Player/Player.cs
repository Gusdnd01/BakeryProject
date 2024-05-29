using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class AnimationClipOverrides : List<KeyValuePair<AnimationClip, AnimationClip>>
{
    public AnimationClipOverrides(int capacity) : base(capacity) { }

    public AnimationClip this[string name]
    {
        get { return this.Find(x => x.Key.name.Equals(name)).Value; }
        set
        {
            int index = this.FindIndex(x => x.Key.name.Equals(name));
            if (index != -1)
                this[index] = new KeyValuePair<AnimationClip, AnimationClip>(this[index].Key, value);
        }
    }
}
public class Player : Entity
{
    private readonly int _moveHash = Animator.StringToHash("Move");
    private readonly int _abilityHash = Animator.StringToHash("Ability");

    public PlayerStat PlayerStat { get; private set; }
    public PlayerVFXManager VFXManager { get; private set; }
    private PlayerHPUI _hpUI;

    private AnimatorOverrideController animatorOverrideController;
    private AnimationClipOverrides clipOverrides;

    public Cream cream;
    private bool _isFront;

    private Dictionary<CardBase, List<Entity>> _saveSkillDic = new();
    public Dictionary<CardBase, List<Entity>> GetSkillTargetEnemyList => _saveSkillDic;

    protected override void Awake()
    {
        base.Awake();

        PlayerStat = CharStat as PlayerStat;
        VFXManager = FindObjectOfType<PlayerVFXManager>();
    }

    public void SaveSkillToEnemy(CardBase skillCard, Entity target)
    {
        if (!_saveSkillDic.ContainsKey(skillCard))
        {
            _saveSkillDic.Add(skillCard, new List<Entity>());
        }
        _saveSkillDic[skillCard].Add(target);
    }

    private void TurnStart(bool b)
    {
        ColliderCompo.enabled = false;
    }
    private void TurnEnd()
    {
        ColliderCompo.enabled = true;
        _saveSkillDic.Clear();
        ChangePosWithCream(false);
        ChainningCardList.Clear();
    }

    protected override void Start()
    {
        base.Start();
        animatorOverrideController = new AnimatorOverrideController(AnimatorCompo.runtimeAnimatorController);
        AnimatorCompo.runtimeAnimatorController = animatorOverrideController;

        ColliderCompo.enabled = true;

        clipOverrides = new AnimationClipOverrides(animatorOverrideController.overridesCount);
        animatorOverrideController.GetOverrides(clipOverrides);

        HealthCompo.OnDeathEvent.AddListener(() => UIManager.Instance.GetSceneUI<BattleUI>().SetResult(false));

        cream.OnAnimationCall = () => OnAnimationCall?.Invoke();
        cream.OnAnimationEnd = () => OnAnimationEnd?.Invoke();

        TurnCounter.PlayerTurnStartEvent += TurnStart;
        TurnCounter.PlayerTurnEndEvent += TurnEnd;
    }

    protected override void OnDisable()
    {
        TurnCounter.PlayerTurnStartEvent -= TurnStart;
        TurnCounter.PlayerTurnEndEvent -= TurnEnd;
        if (_hpUI != null)
            HealthCompo.OnDamageEvent -= _hpUI.SetHpOnUI;
    }

    public void AnimationEndTrigger()
    {
    }

    protected override void HandleDie()
    {
    }

    public override void SlowEntityBy(float percent)
    {
    }

    public void UseAbility(CardBase card, bool isMove = false, bool isCream = false)
    {
        clipOverrides["UseAbility"] = card.CardInfo.abilityAnimation;
        animatorOverrideController.ApplyOverrides(clipOverrides);
        if (!isCream)
        {
            AnimatorCompo.SetBool(_abilityHash, true);
            AnimatorCompo.SetBool(_moveHash, isMove);

            if (isMove)
            {
                MoveToTargetForward(GetSkillTargetEnemyList[card][0].forwardTrm.position);
                if (_isFront) lastMovePos = cream.transform.position;
            }
            ChangePosWithCream(false);
        }
        else
        {
            //ũ�� �ִϸ��̼� ����
            ChangePosWithCream(true, cream.InvokeAnimationCall);
        }
    }

    private void ChangePosWithCream(bool front, Action callback = null)
    {
        if (_isFront == front)
        {
            callback?.Invoke();
            return;
        }

        _isFront = front;
        BattleController.ChangeXPosition(transform, cream.transform, callback);
    }

    public void EndAbility()
    {
        AnimatorCompo.SetBool(_abilityHash, false);
        AnimatorCompo.SetBool(_moveHash, false);
    }

    protected override void HandleEndMoveToTarget()
    {
        AnimatorCompo.SetBool(_moveHash, false);
    }

    protected override void HandleEndMoveToOriginPos()
    {
        // �ϴ� �Ұ� ����
    }
}
