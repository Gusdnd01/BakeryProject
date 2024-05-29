using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct NormalBuff
{
    public StatType type;
    public List<int> values;
}
[System.Serializable]
public struct StackBuff
{
    public StackEnum type;
    public List<int> values;
}


[CreateAssetMenu(menuName = "SO/Buff")]
public class BuffSO : ScriptableObject
{
    private Entity _owner;
    private CharacterStat _stat;

    public List<NormalBuff> statBuffs = new();
    public List<SpecialBuff> specialBuffs = new();
    public List<StackBuff> stackBuffs = new();

    private int _combineLevel = 0;

    public void SetOwner(Entity owner)
    {
        _owner = owner;
        _stat = owner.CharStat;
        specialBuffs.ForEach(b => b.SetOwner(owner));
    }

    public void AppendBuff(int combineLevel = 0)
    {
        _combineLevel = combineLevel;
        foreach (var b in statBuffs)
        {
            _stat.IncreaseStatBy(b.values[combineLevel], _stat.GetStatByType(b.type));
        }
        foreach (var b in specialBuffs)
        {

            _owner.BuffStatCompo.ActivateSpecialBuff(b);
        }
    }
    
    public void RefreshBuff(int combineLevel = 0)
    {
        _combineLevel = combineLevel;
        foreach (var b in statBuffs)
        {
            _stat.IncreaseStatBy(b.values[combineLevel], _stat.GetStatByType(b.type));
        }
        foreach (var b in specialBuffs)
        {
            b.Refresh(_combineLevel);
        }
    }

    public void Update()
    {
        foreach (var b in specialBuffs)
        {
            b.Active(_combineLevel);
        }
    }

    public void PrependBuff()
    {
        foreach (var b in statBuffs)
        {
            _stat.DecreaseStatBy(b.values[_combineLevel], _stat.GetStatByType(b.type));
        }
    }
}
