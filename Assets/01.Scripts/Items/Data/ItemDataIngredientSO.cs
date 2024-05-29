using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName ="SO/Items/Ingredient")]
public class ItemDataIngredientSO : ItemDataSO
{
    public int itemIndex;
    public IngredientType ingredientType;
    public bool isUsed = false;
}
