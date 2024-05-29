using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Items/DropTable")]
public class DropTableSO : ScriptableObject
{
    public List<ItemDataSO> dropList;
    [Range(0, 100f)]
    public float dropChance;

    private float[] _itemWeights;

    private void OnEnable()
    {
        //드롭찬스만 따로 뽑아서 배열로 관리.
        _itemWeights = dropList.Select(item => item.dropChance).ToArray();
    }

    //드랍찬스를 넣으면 해당 찬스안에 들어가면 가능성이 있는 애로 뱉는다. 
    public bool GetDropItem(out ItemDataSO data)
    {
        float value = Random.Range(0, 100f);
        data = null;
        if (value <= dropChance)
        {
            data = dropList[GetRandomWeightedIndex()];
            return true;
        }

        return false;
    }


    private int GetRandomWeightedIndex()
    {
        float sum = 0f;
        for (int i = 0; i < _itemWeights.Length; i++)
        {
            sum += _itemWeights[i];
        }

        float randomValue = Random.Range(0f, sum);
        float tempSum = 0f;

        for (int i = 0; i < _itemWeights.Length; i++)
        {
            if (randomValue >= tempSum && randomValue < tempSum + _itemWeights[i])
            {
                return i;
            }
            else
            {
                tempSum += _itemWeights[i];
            }
        }

        return 0;
    }
}
