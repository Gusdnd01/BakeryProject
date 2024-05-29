using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;
using DG.Tweening;

public class CheckBox : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image _activationMark;
    public Action<bool> OnValueChanged;
    private Tween _scalingTween;
    private bool _isActive;
    public bool IsActive
    {
        get
        {
            return _isActive;
        }
        set
        {
            _isActive = value;
            _activationMark.enabled = _isActive;
            
            if(_isActive)
            {
                _scalingTween.Kill();
                _scalingTween = transform.DOScale(1, 0.2f).SetEase(Ease.OutBack);
            }
            else
            {
                transform.localScale = Vector3.one * 0.8f;
            }

            OnValueChanged?.Invoke(_isActive);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        IsActive = !IsActive;
    }
}
