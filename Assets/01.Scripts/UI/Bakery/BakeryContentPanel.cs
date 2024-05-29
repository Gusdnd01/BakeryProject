using ExtensionFunction;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using static UnityEditor.Progress;

public enum RecipeSortType
{
    Fast = -1,
    Favorites,
    Baking
}

public class BakeryContentPanel : MonoBehaviour
{
    private BakeryFilterTab[] _filterTabArr;

    [SerializeField] private RecipeSortType _startSorting;
    
    [SerializeField] private RecipeElement _recipeElementPrefab;
    [SerializeField] private float _recipeElementInterval;
    [SerializeField] private RectTransform _recipeElementTrm;

    [SerializeField] private IngredientElement _IngredientElementPrefab;
    [SerializeField] private float _ingredientElementInterval;
    [SerializeField] private RectTransform _ingredientElementTrm;

    private Dictionary<RecipeSortType, Action> _recipeSortActionDic = new();

    [SerializeField] private UnityEvent<RecipeSortType> _recipeSortEvent;
    private BakeryData _bakeryData => UIManager.Instance.GetSceneUI<BakeryUI>().BakeryData;

    private void Awake()
    {
        _filterTabArr = GetComponentsInChildren<BakeryFilterTab>();

        foreach(BakeryFilterTab bft in _filterTabArr)
        {
            bft.RecipePanel = this;
        }

        _recipeSortActionDic.Add(RecipeSortType.Fast, FastRecipeSortAction);
        _recipeSortActionDic.Add(RecipeSortType.Baking, NewRecipeSortAction);
        _recipeSortActionDic.Add(RecipeSortType.Favorites, FavoritesRecipeSortAction);
    }

    private void FastRecipeSortAction()
    {
        foreach (CakeData cd in _bakeryData.CakeDataList)
        {
            RecipeElement re = Instantiate(_recipeElementPrefab, _recipeElementTrm);
            re.ThisCakeData = cd;
            re.SetCakeInfo(BakingManager.Instance.GetCakeDataByName(cd.CakeName));
            re.ClickAction += UIManager.Instance.GetSceneUI<BakeryUI>().SelectRecipe;

            _recipeElementTrm.sizeDelta += new Vector2(0, _recipeElementInterval);
        }
    }

    private void NewRecipeSortAction()
    {
        List<ItemDataSO> itemList = Inventory.Instance.GetSpecificTypeItemList(ItemType.Ingredient);
        Action<IngredientElement> selectingAction = UIManager.Instance.GetSceneUI<BakeryUI>().SelectIngredient;

        for(int i = 0; i< itemList.Count; i++)
        {
            if(i % 3 == 0)
            {
                _ingredientElementTrm.sizeDelta += new Vector2(0, _ingredientElementInterval);
            }

            var ingElement = Instantiate(_IngredientElementPrefab, _ingredientElementTrm);
            ingElement.SetInfo(itemList[i] as ItemDataIngredientSO);
            ingElement.SelectThisItemAction += selectingAction;
        }
    }

    private void FavoritesRecipeSortAction()
    {
        List<CakeData> _favorites = 
        _bakeryData.CakeDataList.Where(c => c.IsFavorites).ToList();

        foreach (CakeData cd in _favorites)
        {
            RecipeElement re = Instantiate(_recipeElementPrefab, _recipeElementTrm);
            re.ThisCakeData = cd;
            re.SetCakeInfo(BakingManager.Instance.GetCakeDataByName(cd.CakeName));
            re.ClickAction += UIManager.Instance.GetSceneUI<BakeryUI>().SelectRecipe;

            _recipeElementTrm.sizeDelta += new Vector2(0, _recipeElementInterval);
        }
    }

    private void Start()
    {
        InvokeRecipeAction(_startSorting);
    }

    public void InvokeRecipeAction(RecipeSortType rSortType)
    {
        FilterTabSelectionGenerate(rSortType);

        _recipeElementTrm.Clear();
        _ingredientElementTrm.Clear();

        _recipeSortActionDic[rSortType]?.Invoke();
        _recipeSortEvent?.Invoke(rSortType);
    }

    private void FilterTabSelectionGenerate(RecipeSortType rSortType)
    {
        foreach(BakeryFilterTab bft in _filterTabArr)
        {
            if(bft.SortType != rSortType)
            {
                bft.UnSelected();
            }
        }
    }
}
