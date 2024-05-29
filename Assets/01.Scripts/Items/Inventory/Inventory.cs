using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class Inventory : MonoSingleton<Inventory>
{
    private List<ItemDataSO> _inventoryList = new List<ItemDataSO>();

    public ExpansionList<ItemDataIngredientSO> GetIngredientInThisBattle { get; set; } = 
       new ExpansionList<ItemDataIngredientSO>();

    private void HandleClearGetIngList(Scene arg0, Scene arg1)
    {
        GetIngredientInThisBattle.Clear();
    }

    public List<ItemDataSO> GetSpecificTypeItemList(ItemType itemType)
    {
        return _inventoryList.Where(x => x.itemType == itemType).ToList();
    }

    public bool IsHaveItem(ItemDataSO itemData)
    {
        return _inventoryList.Contains(itemData);
    }

    public void AddItem(ItemDataSO item, int count = 1)
    { 
        if(_inventoryList.Contains(item))
        {
            item.haveCount += count;
            return;
        }

        item.haveCount = 1;
        _inventoryList.Add(item);
    }

    public void RemoveItem(ItemDataSO item, int count = 1)
    {
        if (_inventoryList.Contains(item))
        {
            Mathf.Clamp(item.haveCount -= count, 0, int.MaxValue);
            if(item.haveCount == 0)
            {
                _inventoryList.Remove(item);
            }
        }
        else
        {
            Debug.LogError("아이템 없음!");
        }
    }
}
