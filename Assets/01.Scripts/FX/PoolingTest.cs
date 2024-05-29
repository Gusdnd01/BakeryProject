using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PoolingTest : MonoBehaviour
{
    [SerializeField]
    private PoolListSO poolList;

    [SerializeField]
    private Transform poolTrm;

    private void Awake()
    {
        PoolManager.Instance = new PoolManager(poolTrm);

        foreach (PoolingItem item in poolList.poolList)
        {
            PoolManager.Instance.CreatePool(item.prefab, item.type, item.count);
        }
    }

}
