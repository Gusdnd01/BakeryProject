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
    Forging = 0, // ����
    Lightning, // ����
}

public class Health : MonoBehaviour, IDamageable
{
    public int maxHealth;

    [SerializeField] private int _currentHealth;

    public Action<Color, int> OnDamageText; //������ �ؽ�Ʈ�� ����� �Ҷ�.
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
    private bool _isInvincible = false; //��������
    [SerializeField] private AilmentStat _ailmentStat; //���� �� ����� ���� ����
    public AilmentStat AilmentStat => _ailmentStat;

    public bool isLastHitCritical = false; //������ ������ ũ��Ƽ�÷� �����߳�?

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
        //���⼭ ������ ���ŵ��� �ϵ��� �Ͼ�� �Ѵ�.
        OnAilmentChanged?.Invoke(_ailmentStat.currentAilment);

    }

    public void AilementDamage(AilmentEnum ailment, int damage)
    {
        //������ ���� ���ڰ� �ߵ��� �ؾ��Ѵ�.
        Debug.Log($"{ailment.ToString()} dot damaged : {damage}");
        OnHitEvent?.Invoke();
        _currentHealth = Mathf.Clamp(_currentHealth - damage, 0, maxHealth);
        AfterHitFeedbacks();
    }

    protected void UpdateHealth()
    {
        _ailmentStat.UpdateAilment(); //���� ������Ʈ
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
        if (_isDead || _isInvincible) return; //����ϰų� �������¸� ���̻� ������ ����.
        _currentHealth = Mathf.Clamp(_currentHealth - damage, 0, maxHealth);
    }
    public void ApplyDamage(int damage, Entity dealer, Action action = null)
    {
        if (_isDead || _isInvincible) return; //����ϰų� �������¸� ���̻� ������ ����.


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


        //���⼭ ������ ����ֱ�
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

    //�����̻� �ɱ�.
    public void SetAilment(AilmentEnum ailment, int duration)
    {
        _ailmentStat.ApplyAilments(ailment, duration);

        OnAilmentChanged?.Invoke(_ailmentStat.currentAilment);
    }

    public void AilmentByDamage(AilmentEnum ailment, int damage)
    {
        //��ũ������ �߰� �κ�.
        //������� ������ �ؽ�Ʈ �߰�
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