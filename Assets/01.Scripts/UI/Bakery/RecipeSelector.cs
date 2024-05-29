using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RecipeSelector : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private float _easingTime;
    [SerializeField] private float _scaleUpValue;
    private Tween _hoverTween;

    public void OnPointerEnter(PointerEventData eventData)
    {
        _hoverTween?.Kill();

        _hoverTween = transform.DOScale(Vector3.one * _scaleUpValue, _easingTime); 
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _hoverTween?.Kill();

        _hoverTween = transform.DOScale(Vector3.one, _easingTime);
    }
}
