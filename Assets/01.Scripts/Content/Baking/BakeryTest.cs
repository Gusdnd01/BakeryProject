using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BakeryTest : MonoBehaviour
{
    [SerializeField] private ItemDataIngredientSO[] id;

    private void OnEnable()
    {
        foreach (var item in id)
        {
            Inventory.Instance.AddItem(item, 3);
        }
    }
}
