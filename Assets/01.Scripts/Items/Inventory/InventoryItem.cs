using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class InventoryItem : IComparable<InventoryItem>
{
    public ItemDataSO itemDataSO;
    public int stackSize;

    public InventoryItem(ItemDataSO itemDataSO)
    {
        this.itemDataSO = itemDataSO;
        AddStack(1);
    }

    public void AddStack(int count = 1)
    {
        stackSize += count;
    }

    public int CompareTo(InventoryItem other)
    {
        return other.itemDataSO.itemName.CompareTo(this.itemDataSO.itemName);
    }

    public void RemoveStack(int count)
    {
        stackSize -= count;
    }
}
