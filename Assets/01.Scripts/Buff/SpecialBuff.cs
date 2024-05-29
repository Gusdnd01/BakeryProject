using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class SpecialBuff : ScriptableObject
{
    protected Entity entity;
    protected int combineLevel = 0;

    private bool isComplete = false;
    public void SetOwner(Entity entity)
    {
        this.entity = entity;
    }
    public virtual void Init()
    {
    }
    public virtual void Refresh(int level)
    {
        combineLevel = level;
    }
    public virtual void Active(int level)
    {
        combineLevel = level;
    }
    public virtual void EndBuff() { }
    public virtual void SetIsComplete(bool value)
    {
        isComplete = value;
        if(isComplete == true)
        {
            EndBuff();
            entity.BuffStatCompo.CompleteBuff(this);
        }
    }
}
