using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FavoritesMark : MonoBehaviour, IPointerClickHandler
{
    [Header("ÂüÁ¶")]
    [SerializeField] private Sprite _onFacoriteSprite;
    [SerializeField] private Sprite _notFacoriteSprite;

    private Image _visualImage;
    private bool _isThisRecipeFavorites;
    private RecipeElement _recipeElement;

    public void SetState(RecipeElement recipeElement)
    {
        _recipeElement = recipeElement;
        _visualImage = GetComponent<Image>();

        ChangeStateVisual(_recipeElement.ThisCakeData.IsFavorites);
    }

    private void ChangeStateVisual(bool isFavorites)
    {
        Debug.Log(isFavorites);
        _isThisRecipeFavorites = isFavorites;

        _visualImage.sprite = isFavorites ? _onFacoriteSprite : _notFacoriteSprite;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        ChangeStateVisual(!_isThisRecipeFavorites);

        BakeryData data = DataManager.Instance.LoadData<BakeryData>(DataKeyList.bakeryRecipeDataKey);
        CakeData cd = data.CakeDataList.FirstOrDefault(x => x.CakeName == _recipeElement.ThisCakeData.CakeName);
        cd.IsFavorites = _isThisRecipeFavorites;

        DataManager.Instance.SaveData(data, DataKeyList.bakeryRecipeDataKey);
    }
}
