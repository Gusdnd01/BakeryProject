using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class IngredientElement : MonoBehaviour, IPointerClickHandler
{
    public ItemDataIngredientSO IngredientData { get; private set; }
    public Action<IngredientElement> SelectThisItemAction { get; set; }

    [SerializeField] private Image _visual;
    [SerializeField] private GameObject _selectMask;
    [SerializeField] private TextMeshProUGUI _countText;

    private bool _isSelected;
    public bool IsSelected
    {
        get
        {
            return _isSelected;
        }
        set
        {
            _isSelected = value;
            _selectMask.SetActive(value);
        }
    }

    public void SetInfo(ItemDataIngredientSO ingInfo)
    {
        IngredientData = ingInfo;
        _visual.sprite = ingInfo.itemIcon;
        _countText.text = ingInfo.haveCount.ToString();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (IsSelected) return;

        IsSelected = true;
        SelectThisItemAction?.Invoke(this);
    }
}
