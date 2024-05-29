using System.Collections.Generic;
using UnityEngine;

public class ItemDrop : MonoBehaviour
{
    [SerializeField] private DropTableSO _dropTable;
    [SerializeField] private int _dropDiceCount = 2; //드롭 주사위 굴릴 횟수
    [SerializeField] private ItemObject _dropPrefab;
    public void DropItem(Vector2 dropVelocity)
    {
        for (int i = 0; i < _dropDiceCount; ++i)
        {
            Vector2 velocity = dropVelocity;
            velocity += Random.insideUnitCircle;
            //드롭찬스 안에서 걸리면
            if (_dropTable.GetDropItem(out ItemDataSO item))
            {
                ItemObject newDrop = Instantiate(_dropPrefab, transform.position, Quaternion.identity);
                newDrop.SetUpItem(item, velocity);
            }
        }
    }
}