using UnityEngine;
using UnityEngine.UI;

public class LookBakingPreviewPanel : PreviewPanel
{
    [SerializeField] 
    private Image[] _ingredientElementVisualArr = new Image[3];
    private IngredientElement[] _ingredientElementArr = new IngredientElement[3];

    [SerializeField] private GameObject _cakeImgObj;
    [SerializeField] private GameObject _questionMarkObj;

    protected override void LookUpContent()
    {
        for(int i = 0; i < _ingredientElementArr.Length; i++)
        {
            _ingredientElementVisualArr[i].enabled = false;
            _ingredientElementArr[i] = null;
        }
    }

    public void SetIngredientElement(IngredientElement ingElement)
    {
        int idx = (int)ingElement.IngredientData.ingredientType;

        if (_ingredientElementArr[idx] != null)
        {
            _ingredientElementArr[idx].IsSelected = false;
        }

        _ingredientElementArr[idx] = ingElement;
        var element = _ingredientElementVisualArr[idx];

        element.enabled = true;
        element.sprite = ingElement.IngredientData.itemIcon;
    }

    public void BakeCake()
    {
        ItemDataIngredientSO[] ingDatas =
            {
                _ingredientElementArr[0].IngredientData,
                _ingredientElementArr[1].IngredientData,
                _ingredientElementArr[2].IngredientData,
            };

        if (BakingManager.Instance.CanBake(ingDatas))
        {
            foreach(var ingData in ingDatas)
            {
                Inventory.Instance.RemoveItem(ingData);
            }

            ItemDataBreadSO cake = BakingManager.Instance.BakeBread(ingDatas);
            Inventory.Instance.AddItem(cake);

            BakeryUI bui = UIManager.Instance.GetSceneUI<BakeryUI>();

            CakeData cakeData = new CakeData(cake.itemName, false);
            if (!bui.BakeryData.CakeDataList.Contains(cakeData))
            {
                bui.BakeryData.CakeDataList.Add(cakeData);
                bui.SaveData();
            }

            bui.GetCakePanel.SetUp(cake);
            bui.FilteringPreviewContent(MySortType);
            bui.RecipePanel.InvokeRecipeAction(MySortType);
        }
    }
}
