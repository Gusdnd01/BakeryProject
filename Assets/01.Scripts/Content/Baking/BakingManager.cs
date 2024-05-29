using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public enum IngredientType
{
    Core,       // 海捞胶
    Trace,     // 如利
    Subjectivity,     // 林包
}

public struct CakeGroup
{
    public string[] ingDatas;
    public ItemDataBreadSO cake;

    public CakeGroup(string[] _data, ItemDataBreadSO _cake)
    {
        ingDatas = _data;
        cake = _cake;
    }
}

public class BakingManager : MonoSingleton<BakingManager>
{
    [SerializeField] private BreadRecipeTable _recipeTable;

    private Dictionary<string, ItemDataBreadSO> _cakeDictionary = new();
    [SerializeField] private List<ItemDataBreadSO> _breadList = new();

    private Dictionary<string, ItemDataIngredientSO> _ingredientDic = new();
    [SerializeField] private List<ItemDataIngredientSO> _ingredientList = new();

    private List<CakeGroup> _cakeGroupList = new List<CakeGroup>();

    private void Start()
    {
        foreach (ItemDataBreadSO cake in _breadList)
        {
            _cakeDictionary.Add(cake.itemName, cake);
        }

        foreach (ItemDataIngredientSO ing in _ingredientList)
        {
            _ingredientDic.Add(ing.itemName, ing);
        }

        foreach (Data data in _recipeTable.DataList)
        {
            string[] extractedData = data.str;
            string[] ida = new string[3] { extractedData[1], extractedData[2], extractedData[3] };
            Array.Sort(ida);

            CakeGroup cakeGroup =
            new CakeGroup
            (
                ida,
                GetCakeDataByName(extractedData[0])
            );

            _cakeGroupList.Add(cakeGroup);
        }
    }

    public bool CanBake(ItemDataIngredientSO[] ingredients)
    {
        foreach(ItemDataIngredientSO i in ingredients)
        {
            if(i == null) return false;
        }

        return true;
    }
    public ItemDataBreadSO BakeBread(ItemDataIngredientSO[] ingredients)
    {
        if (!CanBake(ingredients))
        {
            Debug.LogError("Plz Check Can Bake");
            return null;
        }

        string[] ingNames = ingredients.Select(x => x.itemName).ToArray();
        Array.Sort(ingNames);

        CakeGroup cg = _cakeGroupList.FirstOrDefault(x => 
        x.ingDatas[0] == ingNames[0] &&
        x.ingDatas[1] == ingNames[1] &&
        x.ingDatas[2] == ingNames[2]);

        ItemDataBreadSO returnBread;

        if(cg.Equals(default(CakeGroup)))
        {
            returnBread = _cakeDictionary["DubiousBread"];
        }
        else
        {
            returnBread = cg.cake;
        }

        return returnBread;
    }
    public ItemDataBreadSO GetCakeDataByName(string cakeName)
    {
        if(!_cakeDictionary.ContainsKey(cakeName))
        {
            Debug.LogError($"{cakeName} is Not Exist!");
            return null;
        }

        return _cakeDictionary[cakeName];
    }

    public ItemDataIngredientSO GetIngredientDataByName(string ingredientName)
    {
        if (!_ingredientDic.ContainsKey(ingredientName))
        {
            Debug.LogError($"{ingredientName} is Not Exist");
            return null;
        }

        return _ingredientDic[ingredientName];
    }

    public ItemDataIngredientSO[] GetIngredientDatasByCakeName(string cakeName)
    {
        string[] ingNames = _recipeTable.GetIngredientNamesByCakeName(cakeName);

        ItemDataIngredientSO[] datas = new ItemDataIngredientSO[3]
        {
            GetIngredientDataByName(ingNames[0]),
            GetIngredientDataByName(ingNames[1]),
            GetIngredientDataByName(ingNames[2])
        };

        return datas;
    }

    [ContextMenu("GET_ALL_INGREDIENT")]
    public void TEST_Get_All_Ingredient_Item()
    {
        foreach(var item in _ingredientList)
        {
            Inventory.Instance.AddItem(item);
        }
    }
}