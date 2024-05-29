using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreadStash : Stash
{
    public BreadStash(Transform slotParentTrm) : base(slotParentTrm)
    {
    }

    public override void AddItem(ItemDataSO item, int count = 1)
    {
        //중복된 아이템은 CanAddItem에서 걸러줄꺼니까 여기서 걸르지 않는다.
        InventoryItem newItem = new InventoryItem(item);
        stash.Add(newItem);
        stashDictionary.Add(item, newItem);
    }

    public override void RemoveItem(ItemDataSO item, int count = 1)
    {
        if (stashDictionary.TryGetValue(item, out InventoryItem invenItem))
        {
            stash.Remove(invenItem);
            stashDictionary.Remove(item);
        }
    }

    public override bool CanAddItem(ItemDataSO itemData)
    {
        if (stashDictionary.ContainsKey(itemData))
        {
            Debug.Log("No more space or already exists!");
            return false;
        }
        return true;
    }

}