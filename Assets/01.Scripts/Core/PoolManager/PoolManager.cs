using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PoolingType
{
    DamageText,
    DialogueEffect,
    SwordAura,
    HoneyTreeSpike,
    IngredientItemElement,
    ItemInfoPopUpPanel,
    ItemElement,
    HitEffect,
    CardInfoPanel,
    CandyParty,
    ErrorText,
    TestElement,
    Mr_Muddy,
    MonStrowBerry,
    PumpkinBat,
    Sound,
    KingBufferDog,
    RiceBird,
    TouchEffect,
    SeedGun,
    VCamPool,
    CandyStreamBurst,
    CardLevelUpFx,
    FallingThunder,
    Pumpkin,
    Boomerang,
    Tornado,
    Boss
}
public enum ParticleEnum
{

}

public class PoolManager
{
    public static PoolManager Instance;

    private Dictionary<PoolingType, Pool<PoolableMono>> _poolDic = new Dictionary<PoolingType, Pool<PoolableMono>>();
    //private Dictionary<PoolingType, PoolGroup> Dic = new();
    

    private Transform _parentTrm;
    public PoolManager(Transform parentTrm)
    {
        _parentTrm = parentTrm;
    }
    
    public void CreatePool(PoolableMono prefab, PoolingType poolingType, int count = 10)
    {
        _poolDic.Add(poolingType, new Pool<PoolableMono>(prefab, poolingType, _parentTrm, count));
    }
    public void Push(PoolableMono obj)
    {
        if (_poolDic.ContainsKey(obj.poolingType))
            _poolDic[obj.poolingType].Push(obj);
        else
            Debug.LogError($"not have ${obj.name} pool");
    }
    public PoolableMono Pop(PoolingType type)
    {
        PoolableMono obj = null;
        if (!_poolDic.ContainsKey(type))
        {
            Debug.LogError($"not have [${type.ToString()}] pool");
        }
        obj = _poolDic[type].Pop();
        obj.Init();
        return obj;
    }
    public bool Contains(PoolableMono obj)
    {
        if (_poolDic.ContainsKey(obj.poolingType))
            return _poolDic[obj.poolingType].Contain(obj);
        return false;
    }
}
