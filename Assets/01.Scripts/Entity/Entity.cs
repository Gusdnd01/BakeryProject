using Cinemachine;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif
using UnityEngine;
using UnityEngine.Events;

public abstract class Entity : PoolableMono
{
    #region components
    public Animator AnimatorCompo { get; private set; }
    public Health HealthCompo { get; private set; }
    public SpriteRenderer SpriteRendererCompo { get; private set; }
    public BattleController BattleController { get; set; }
    public BuffStat BuffStatCompo { get; private set; }
    [field: SerializeField] public CharacterStat CharStat { get; private set; }

    public Collider2D ColliderCompo { get; private set; }
    #endregion

    protected int _hitAnimationHash = Animator.StringToHash("hit");
    protected int _deathAnimationHash = Animator.StringToHash("death");

    public UnityEvent<float> OnHealthBarChanged;
    public Action OnAnimationCall;
    public Action OnAnimationEnd;

    public Action OnMoveTarget;
    public Action OnMoveOriginPos;

    public Action OnAttackStart;
    public Action OnAttackEnd;

    public List<IOnTakeDamage> OnAttack = new();

    [Header("셋팅값들")]
    public Transform hpBarPos;
    public Transform forwardTrm;

    public Entity target;

    [SerializeField] protected Vector3 lastMovePos;
    [SerializeField] protected float moveDuration = 0.1f;

    public TurnStatus turnStatus;

    public UnityEvent BeforeChainingEvent => CardReader.SkillCardManagement.beforeChainingEvent;

    public List<CardBase> ChainningCardList { get; set; } = new List<CardBase>();

    private Tween _materialChangeTween;

    public BuffingMarkSetter BuffSetter { get; set; }

    public void SelectChainningCharacter(Color skillColor, float Thickness)
    {
        _materialChangeTween.Kill();

        Material mat = new Material(SpriteRendererCompo.material);
        SpriteRendererCompo.material = mat;

        mat.SetFloat("_outline_thickness", 0);
        mat.SetColor("_outline_color", skillColor);

        _materialChangeTween = mat.DOFloat(Thickness, "_outline_thickness", 0.2f);
    }

    protected virtual void Awake()
    {
        Transform visualTrm = transform.Find("Visual");
        AnimatorCompo = visualTrm.GetComponent<Animator>();
        HealthCompo = GetComponent<Health>();
        SpriteRendererCompo = visualTrm.GetComponent<SpriteRenderer>();
        HealthCompo.SetOwner(this);

        CharStat = Instantiate(CharStat); //������ ����
        CharStat.SetOwner(this);

        ColliderCompo = GetComponent<Collider2D>();

        BuffStatCompo = new BuffStat(this);
    }

    protected virtual void Start()
    {
    }
    protected virtual void OnEnable()
    {
        HealthCompo.SetOwner(this);

        TurnCounter.RoundStartEvent += BuffStatCompo.UpdateBuff;

        OnMoveTarget += HandleEndMoveToTarget;
        OnMoveOriginPos += HandleEndMoveToOriginPos;
        HealthCompo.OnHitEvent.AddListener(HandleHit);

        HealthCompo.OnAilmentChanged.AddListener(HandleAilmentChanged);
        OnHealthBarChanged?.Invoke(HealthCompo.GetNormalizedHealth()); //�ִ�ġ�� UI����.

        HealthCompo.OnDeathEvent.AddListener(HandleDie);
        HealthCompo.OnDeathEvent.AddListener(BuffStatCompo.ClearStat);
        ColliderCompo.enabled = true;
    }
    protected virtual void OnDisable()
    {
        BuffStatCompo.ClearStat();

        OnMoveTarget -= HandleEndMoveToTarget;
        OnMoveOriginPos -= HandleEndMoveToOriginPos;

        HealthCompo.OnDeathEvent.RemoveListener(HandleDie);
        HealthCompo.OnDeathEvent.RemoveListener(BuffStatCompo.ClearStat);

        HealthCompo.OnAilmentChanged.RemoveListener(HandleAilmentChanged);

        HealthCompo.OnHitEvent.RemoveListener(HandleHit);
    }

    private void OnDestroy()
    {
        HealthCompo.OnAilmentChanged.RemoveListener(HandleAilmentChanged);
    }

    //���ῡ ���� ó��.
    private void HandleAilmentChanged(AilmentEnum ailment)
    {
        if ((ailment & AilmentEnum.Chilled) > 0) //������¸� ���ǵ� ������
        {
            //���� ���׿� ���� ����
            float resistance = (100 - CharStat.armor.GetValue()) * 0.01f;
            SlowEntityBy(0.5f * resistance);
        }
        else
        {
            ReturnDefaultSpeed();
        }
    }

    protected virtual void HandleHit()
    {
        //UI����
        float currentHealth = HealthCompo.GetNormalizedHealth();
        if (currentHealth > 0)
        {
            AnimatorCompo.SetTrigger(_hitAnimationHash);
        }

        OnHealthBarChanged?.Invoke(currentHealth);
    }

    protected virtual void HandleDie()
    {
        EnemyStat es = CharStat as EnemyStat;
        Inventory.Instance.GetIngredientInThisBattle.Add(es.DropItem);
        Inventory.Instance.AddItem(es.DropItem);
        AnimatorCompo.SetTrigger(_deathAnimationHash);
    }

    public abstract void SlowEntityBy(float percent); //���ο�� �ڽĵ��� ����.

    protected virtual void ReturnDefaultSpeed()
    {
        AnimatorCompo.speed = 1; //���� ���ǵ�� �ǵ�����.
    }

    public virtual void FreezeTime(bool isFreeze, bool isFrozenWithoutTimer = false)
    {
        if (isFreeze)
        {
            Debug.Log("Freezed");
            AnimatorCompo.speed = 0; //�ִϸ��̼� ����. �̵� ����.
        }
        else
        {
            Debug.Log("UnFreezed");
            AnimatorCompo.speed = 1;
        }
    }

    public virtual void MoveToTargetForward(Vector3 pos)
    {
        lastMovePos = transform.position;


        Sequence seq = DOTween.Sequence();
        //seq.Append(transform.DOMove(target.forwardTrm.position, moveDuration));
        seq.Append(transform.DOJump(target.forwardTrm.position, 1,1,0.6f));
        seq.OnComplete(OnMoveTarget.Invoke);
    }
    protected abstract void HandleEndMoveToTarget();
    public virtual void MoveToOriginPos()
    {
        transform.DOMove(lastMovePos, moveDuration).OnComplete(OnMoveOriginPos.Invoke);
    }
    protected abstract void HandleEndMoveToOriginPos();

    public void DeadSequence()
    {
        CardReader.SkillCardManagement.useCardEndEvnet.RemoveListener(DeadSequence);
        HealthCompo.OnDeathEvent?.Invoke();
        GotoPool();
    }
    public void GotoPool()
    {
        StartCoroutine(DissolveCo());
    }
    private IEnumerator DissolveCo()
    {
        float timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime;
            SpriteRendererCompo.material.SetFloat("_dissolve_amount",Mathf.Lerp(0,1,timer));
            yield return null;
        }
        HealthCompo.OnDeathEvent.RemoveAllListeners();
        if(this is not Player)
            PoolManager.Instance.Push(this);
    }

    public override void Init()
    {

    }
}
