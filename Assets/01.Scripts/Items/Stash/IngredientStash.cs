using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class IngredientStash : Stash
{
    public IngredientStash(Transform slotParentTrm) : base(slotParentTrm)
    {
    }

    public override void AddItem(ItemDataSO item, int count = 1)
    {
        if (stashDictionary.TryGetValue(item, out InventoryItem invenItem))
        {
            Debug.Log("Add Item");
            invenItem.AddStack(count);
        }
        else
        {
            Debug.Log("Add New Item");
            InventoryItem newItem = new InventoryItem(item);
            newItem.AddStack(count - 1);
            stash.Add(newItem);
            stashDictionary.Add(item, newItem);
        }
    }

    public override bool CanAddItem(ItemDataSO item)
    {
        if (!stashDictionary.ContainsKey(item))
        {
            Debug.Log("Full");
            return false;
        }
        return true;
    }

    public override void RemoveItem(ItemDataSO item, int count = 1)
    {
        if (stashDictionary.TryGetValue(item, out InventoryItem invenItem))
        {
            if (invenItem.stackSize <= count)
            {
                stash.Remove(invenItem);
                stashDictionary.Remove(item);
            }
            else
            {
                invenItem.RemoveStack(count);
            }
        }
    }
}
