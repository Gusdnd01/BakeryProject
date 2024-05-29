using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsedIngredientStash : Stash
{
    public Dictionary<IngredientType, InventoryItem> usedIngredDictionary;
    public List<InventoryItem> usedIngredientStash;

    public UsedIngredientStash(Transform slotParentTrm) : base(slotParentTrm)
    {
        usedIngredientStash = new List<InventoryItem>();
        usedIngredDictionary = new Dictionary<IngredientType, InventoryItem>();

        _slotParentTrm = slotParentTrm;
    }

    public override void AddItem(ItemDataSO item, int count = 1)
    {
        // 그냥 Dictionary key만 enum으로 바꾼다.
        // CanAddItem에서 같은 분류의 재료가 이미 들어가  있는지 확인헀기 때문에 그냥 넣음
        InventoryItem newItem = new InventoryItem(item);
        //stash.Add(newItem);
        //usedIngredientStash.Add(newItem);
        usedIngredientStash[(int)((ItemDataIngredientSO)item).ingredientType] = newItem;
        //Debug.Log((int)((ItemDataIngredientSO)item).ingredientType - 1);

        usedIngredDictionary.Add(((ItemDataIngredientSO)item).ingredientType, newItem);
    }

    public override bool CanAddItem(ItemDataSO item)
    {
        // 만약 추가하려는 재료와 같은 분류의 재료가 들어가 있을 경우 넣을 수 없음
        if (usedIngredDictionary.TryGetValue(((ItemDataIngredientSO)item).ingredientType, out InventoryItem invenItem))
        {
            return false;
        }
        return true;
    }

    public override void RemoveItem(ItemDataSO item, int count = 1)
    {
        // 만약 해당 분류의 재료가 들어있으면
        if (usedIngredDictionary.TryGetValue(((ItemDataIngredientSO)item).ingredientType, out InventoryItem invenItem))
        {
            // stash에서 지우고
            //stash.Remove(invenItem);
            //usedIngredientStash.Remove(invenItem);
            ((ItemDataIngredientSO)item).isUsed = false;
            //Debug.Log(((ItemDataIngredientSO)item).isUsed);
            usedIngredientStash[(int)((ItemDataIngredientSO)item).ingredientType] = null;
            // Dictionary에서 지움
            usedIngredDictionary.Remove(((ItemDataIngredientSO)item).ingredientType);
        }

        Inventory.Instance.AddItem(item, count);
    }

    public void RemoveAllItem()
    {
        for(int i = 0; i < 3; ++i)
        {
            int result = (int)Mathf.Pow(2, i);
            ItemDataIngredientSO id = (ItemDataIngredientSO)usedIngredientStash[result].itemDataSO;
            id.isUsed = false;
            usedIngredDictionary.Remove(id.ingredientType);
            

            usedIngredientStash[i] = null;
        }
    }
}
