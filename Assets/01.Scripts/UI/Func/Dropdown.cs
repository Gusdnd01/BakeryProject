using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;
using DG.Tweening;
using TMPro;

public class Dropdown : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private TextMeshProUGUI _label;
    [SerializeField] private float _easingTime;
    [SerializeField] private List<string> _itemInfoList = new List<string>();

    public Action<int> OnValueChanged;
    private int _selectItemIdx;
    public int Value => _selectItemIdx;

    public void SetItem(int value)
    {
        if(value >= _itemInfoList.Count)
        {
            Debug.LogError($"{value} is bigger then ItemList Count. Plz Check");
            return;
        }

        _selectItemIdx = value;
        OnValueChanged?.Invoke(_selectItemIdx);
        _label.text = _itemInfoList[_selectItemIdx];
        _label.transform.DOScale(Vector2.one * 1.2f, _easingTime).
                         OnComplete(() =>
                         {
                             _label.transform.DOScale(Vector2.one, _easingTime);
                         });
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        int idx = _selectItemIdx + 1;
        if(idx >= _itemInfoList.Count)
        {
            idx = 0;
        }

        SetItem(idx);
    }
}
