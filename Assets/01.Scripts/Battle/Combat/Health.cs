using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

[Flags]
public enum AilmentEnum : int
{
    None = 0,
    Chilled = 1,
    Shocked = 2
}

public enum StackEnum : int
{
    Forging = 0, // 단조
    Lightning, // 번개
}

public class Health : MonoBehaviour, IDamageable
{
    public int maxHealth;

    [SerializeField] private int _currentHealth;

    public Action<Color, int> OnDamageText; //데미지 텍스트를 띄워야 할때.
    public Action<float, float> OnDamageEvent;

    public Action OnBeforeHit;
    public UnityEvent OnDeathEvent;
    public UnityEvent OnHitEvent;
    public UnityEvent<AilmentEnum> OnAilmentChanged;

    private Entity _owner;
    [SerializeField] private bool _isDead = false;
    public bool IsDead
    {
        get => _isDead;
        set
        {
            _isDead = value;
        }
    }
    private bool _isInvincible = false; //무적상태
    [SerializeField] private AilmentStat _ailmentStat; //질병 및 디버프 관리 스탯
    public AilmentStat AilmentStat => _ailmentStat;

    public bool isLastHitCritical = false; //마지막 공격이 크리티컬로 적중했냐?

    public bool IsFreeze;

    protected void Awake()
    {
        _ailmentStat = new AilmentStat(this);


    }
    private void OnEnable()
    {
        TurnCounter.RoundEndEvent += UpdateHealth;
        _ailmentStat.EndOFAilmentEvent += HandleEndOfAilment;

        _isDead = false;
    }
    private void OnDisable()
    {
        _ailmentStat.EndOFAilmentEvent -= HandleEndOfAilment;
        TurnCounter.RoundEndEvent -= UpdateHealth;
    }

    private void HandleEndOfAilment(AilmentEnum ailment)
    {
        Debug.Log($"{gameObject.name} : cure from {ailment.ToString()}");
        //여기서 아이콘 제거등의 일들이 일어나야 한다.
        OnAilmentChanged?.Invoke(_ailmentStat.currentAilment);

    }

    public void AilementDamage(AilmentEnum ailment, int damage)
    {
        //종류에 맞춰 글자가 뜨도록 해야한다.
        Debug.Log($"{ailment.ToString()} dot damaged : {damage}");
        OnHitEvent?.Invoke();
        _currentHealth = Mathf.Clamp(_currentHealth - damage, 0, maxHealth);
        AfterHitFeedbacks();
    }

    protected void UpdateHealth()
    {
        _ailmentStat.UpdateAilment(); //질병 업데이트
    }

    public void SetOwner(Entity owner)
    {
        _owner = owner;
        _currentHealth = maxHealth = _owner.CharStat.GetMaxHealthValue();
    }

    public float GetNormalizedHealth()
    {
        if (maxHealth <= 0) return 0;
        return Mathf.Clamp((float)_currentHealth / maxHealth, 0, 1f);
    }

    public void ApplyHeal(int amount)
    {
        _currentHealth = Mathf.Min(_currentHealth + amount, maxHealth);
        Debug.Log($"{_owner.gameObject.name} is healed!! : {amount}");
        _owner.OnHealthBarChanged?.Invoke(GetNormalizedHealth());
    }

    public void ApplyTrueDamage(int damage)
    {
        if (_isDead || _isInvincible) return; //사망하거나 무적상태면 더이상 데미지 없음.
        _currentHealth = Mathf.Clamp(_currentHealth - damage, 0, maxHealth);
    }
    public void ApplyDamage(int damage, Entity dealer, Action action = null)
    {
        if (_isDead || _isInvincible) return; //사망하거나 무적상태면 더이상 데미지 없음.


        if (dealer.CharStat.IsCritical(ref damage))
        {
            isLastHitCritical = true;
        }
        else
        {
            isLastHitCritical = false;
        }
        _owner.BuffStatCompo.OnHitDamageEvent?.Invoke(dealer, ref damage);

        damage = _owner.CharStat.ArmoredDamage(damage, IsFreeze);
        DamageTextManager.Instance.PopupDamageText(_owner.transform.position, damage, isLastHitCritical ? DamageCategory.Critical : DamageCategory.Noraml);
        foreach (var b in dealer.OnAttack)
        {
            b?.TakeDamage(this, ref damage);
        }
        if (_owner.CharStat.CanEvasion())
        {
            Debug.Log($"{_owner.gameObject.name} is evasion attack!");
            return;
        }


        _currentHealth = Mathf.Clamp(_currentHealth - damage, 0, maxHealth);
        _owner.BuffStatCompo.OnHitDamageAfterEvent?.Invoke(dealer, this, ref damage);
        OnDamageEvent?.Invoke(_currentHealth, maxHealth);


        //여기서 데미지 띄워주기
        //DamageTextManager.Instance.PopupReactionText(_owner.transform.position, isLastHitCritical ? DamageCategory.Critical : DamageCategory.Noraml);


        AfterHitFeedbacks();

        action?.Invoke();
    }

    private void AfterHitFeedbacks()
    {
        OnHitEvent?.Invoke();
    }

    [ContextMenu("Chilled")]
    private void Test1()
    {
        SetAilment(AilmentEnum.Chilled, 2);
    }
    [ContextMenu("Shocked")]
    private void Test2()
    {
        SetAilment(AilmentEnum.Shocked, 2);
    }
    [ContextMenu("asdf")]
    private void Test3()
    {
        print("asdf");
        TurnCounter.ChangeRound();
    }

    //상태이상 걸기.
    public void SetAilment(AilmentEnum ailment, int duration)
    {
        _ailmentStat.ApplyAilments(ailment, duration);

        OnAilmentChanged?.Invoke(_ailmentStat.currentAilment);
    }

    public void AilmentByDamage(AilmentEnum ailment, int damage)
    {
        //쇼크데미지 추가 부분.
        //디버프용 데미지 텍스트 추가
        DamageTextManager.Instance.PopupDamageText(_owner.transform.position, damage, DamageCategory.Debuff);
        OnDamageEvent?.Invoke(_currentHealth, maxHealth);
        //Debug.Log($"{gameObject.name} : shocked damage added = {shockDamage}");
    }


    public void MakeInvincible(bool value)
    {
        _isInvincible = value;
    }
    [ContextMenu("TestHitFeedback")]
    private void TestDead()
    {
        AfterHitFeedbacks();
    }
}