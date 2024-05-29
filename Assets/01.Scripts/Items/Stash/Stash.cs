using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class Stash
{
    public List<InventoryItem> stash; //창고에 있는 아이템들의 리스트
    public Dictionary<ItemDataSO, InventoryItem> stashDictionary;

    protected Transform _slotParentTrm;

    public Stash(Transform slotParentTrm)
    {
        stash = new List<InventoryItem>();
        stashDictionary = new Dictionary<ItemDataSO, InventoryItem>();

        _slotParentTrm = slotParentTrm;
    }

    public virtual bool HasItem(ItemDataSO item)
    {
        return stashDictionary.ContainsKey(item);
    }

    public abstract void AddItem(ItemDataSO item, int count = 1);
    public abstract void RemoveItem(ItemDataSO item, int count = 1);
    public abstract bool CanAddItem(ItemDataSO item);

}