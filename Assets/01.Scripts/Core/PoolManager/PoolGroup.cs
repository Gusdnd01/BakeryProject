using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolGroup<T> where T :Enum
{
    //private Dictionary<T, Pool<PoolableMono>> _poolDic = new Dictionary<T, Pool<PoolableMono>>();
    //public void Push(PoolableMono obj)
    //{
    //    if (_poolDic.ContainsKey(obj.poolingType))
    //        _poolDic[obj.poolingType].Push(obj);
    //    else
    //        Debug.LogError($"not have ${obj.name} pool");
    //}
    //public PoolableMono Pop(Enum type)
    //{
    //    PoolableMono obj = null;
    //    if (!_poolDic.ContainsKey(type))
    //    {
    //        Debug.LogError($"not have [${type.ToString()}] pool");
    //    }
    //    obj = _poolDic[type].Pop();
    //    obj.Init();
    //    return obj;
    //}
}
