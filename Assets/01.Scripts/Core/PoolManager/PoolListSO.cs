using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct PoolingItem
{
    public PoolingType type;
    public PoolableMono prefab;
    public int count;
}

[CreateAssetMenu(menuName = "SO/Setting/PoolList")]
public class PoolListSO : ScriptableObject
{
    public List<PoolingItem> poolList;
}
