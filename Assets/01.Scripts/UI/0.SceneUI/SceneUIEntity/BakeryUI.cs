using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UIDefine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class BakeryUI : SceneUI
{
    private BakeryContentPanel _recipePanel;
    public BakeryContentPanel RecipePanel
    {
        get
        {
            if(_recipePanel == null)
            {
                _recipePanel = FindObjectOfType<BakeryContentPanel>();
            }
            return _recipePanel;
        }
    }

    [SerializeField] private GetCakePanel _getCakePanel;
    public GetCakePanel GetCakePanel => _getCakePanel;

    [SerializeField] private GameObject _previewPanelObj;
    private PreviewPanel[] _previewPanels;

    public BakeryData BakeryData
    {
        get
        {
            return DataManager.Instance.LoadData<BakeryData>(DataKeyList.bakeryRecipeDataKey);
        }
    }

    public override void SceneUIStart()
    {
        _previewPanels = _previewPanelObj.GetComponentsInChildren<PreviewPanel>();
    }
    public void SaveData()
    {
        DataManager.Instance.SaveData(BakeryData, DataKeyList.bakeryRecipeDataKey);
    }
    public void FilteringPreviewContent(RecipeSortType type)
    {
        foreach (var panel in _previewPanels)
        {
            panel.SetUpPanel(type);
        }
    }
    public void SelectRecipe(RecipeElement element)
    {
        var bp = _previewPanels.FirstOrDefault(x => x.MySortType == RecipeSortType.Fast) as LookRecipePreviewPanel;
        bp.HandleAppearRecipe(element);
    }
    public void SelectIngredient(IngredientElement element)
    {
        var bp = _previewPanels.FirstOrDefault(x => x.MySortType == RecipeSortType.Baking) as LookBakingPreviewPanel;
        bp.SetIngredientElement(element);
    }
}
