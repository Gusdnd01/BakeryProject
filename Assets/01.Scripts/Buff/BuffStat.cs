using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public delegate void OnHitDamage<T1, T2>(T1 t1, ref T2 t2);
public delegate void OnHitDamageAfter<T1, T2, T3>(T1 dealer, T2 health, ref T3 damage);
public class BuffStat
{
    public AilmentEnum currentAilment;

    public OnHitDamage<Entity, int> OnHitDamageEvent;
    public OnHitDamageAfter<Entity, Health, int> OnHitDamageAfterEvent;

    public List<SpecialBuff> specialBuffList = new();
    private Entity _owner;
    private Dictionary<BuffSO, int> _buffDic = new();
    private Dictionary<StackEnum, int> _stackDic = new();

    public BuffStat(Entity entity)
    {
        _owner = entity;
        _buffDic = new();
        foreach (StackEnum t in Enum.GetValues(typeof(StackEnum)))
        {
            _stackDic.Add(t, 0);
        }
        //_owner.BeforeChainingEvent.AddListener(UpdateBuff);
    }
    public void AddBuff(BuffSO so, int durationTurn, int combineLevel = 0)
    {
        so.SetOwner(_owner);
        if (_buffDic.ContainsKey(so))
        {
            so.PrependBuff();
            so.RefreshBuff();
            _buffDic[so] = durationTurn;
        }
        else
        {
            so.AppendBuff(combineLevel);
            _buffDic.Add(so, durationTurn);
        }
    }
    public void AddStack(StackEnum type, int cnt)
    {
        _stackDic[type] += cnt;
    }
    public int GetStack(StackEnum type) => _stackDic[type];
    public void RemoveStack(StackEnum type, int cnt)
    {
        _stackDic[type] -= cnt;
    }
    public void ClearStack(StackEnum type) => _stackDic[type] = 0;
    public void ActivateSpecialBuff(SpecialBuff buff)
    {
        specialBuffList.Add(buff);
        buff.Init();
        switch (buff)
        {
            case IOnTakeDamage i:
                {
                    if (!_owner.OnAttack.Contains(i))
                        _owner.OnAttack.Add(i);
                }
                break;
            case IOnHitDamage i:
                {
                    OnHitDamageEvent += i.HitDamage;
                }
                break;
            case IOnEndSkill i:
                {
                    CardReader.SkillCardManagement.useCardEndEvnet.AddListener(i.EndSkill);
                }
                break;
            case IOnHitDamageAfter i:
                {
                    OnHitDamageAfterEvent += i.HitDamageAfter;
                }
                break;
        }
    }
    public void CompleteBuff(SpecialBuff special)
    {
        switch (special.GetType())
        {
            case IOnTakeDamage i:
                {
                    if (_owner.OnAttack.Contains(i))
                        _owner.OnAttack.Remove(i);
                }
                break;
            case IOnHitDamage i:
                {
                    OnHitDamageEvent -= i.HitDamage;
                }
                break;
            case IOnEndSkill i:
                {
                    CardReader.SkillCardManagement.useCardEndEvnet.RemoveListener(i.EndSkill);
                }
                break;
            case IOnHitDamageAfter i:
                {
                    OnHitDamageAfterEvent -= i.HitDamageAfter;
                }
                break;
        }
        specialBuffList.Remove(special);
    }

    public void UpdateBuff()
    {
        foreach (var d in _buffDic.Keys.ToList())
        {
            d.Update();

            _buffDic[d]--;
            if (_buffDic[d] <= 0)
            {
                d.PrependBuff();
                _buffDic.Remove(d);
                continue;
            }
        }
    }
    public void ClearStat()
    {
        foreach (var d in _buffDic.Keys.ToList())
        {
            d.PrependBuff();
            _buffDic.Remove(d);
        }
        while (specialBuffList.Count > 0)
        {
            CompleteBuff(specialBuffList[0]);
        }
        TurnCounter.RoundStartEvent -= UpdateBuff;
    }
}
