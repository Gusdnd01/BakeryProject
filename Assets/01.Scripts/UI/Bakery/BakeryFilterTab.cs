using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BakeryFilterTab : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private RecipeSortType _sortType;
    public RecipeSortType SortType => _sortType;
    public BakeryContentPanel RecipePanel { get; set; }

    private Image _filterTabVisual;
    [SerializeField] private Color _selectedColor;
    [SerializeField] private Sprite _selectedSprite;
    private Sprite _normalSprite;

    private void Awake()
    {
        _filterTabVisual = GetComponent<Image>();
        _normalSprite = _filterTabVisual.sprite;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        RecipePanel.InvokeRecipeAction(_sortType);
        _filterTabVisual.color = _selectedColor;
        _filterTabVisual.sprite = _selectedSprite;
    }

    public void UnSelected()
    {
        _filterTabVisual.color = Color.white;
        _filterTabVisual.sprite = _normalSprite;
    }
}
