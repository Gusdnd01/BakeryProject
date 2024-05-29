using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class ElementBubble : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Action OnPointerClickEvent { get; set; }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnPointerClickEvent?.Invoke();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.DOKill();
        transform.DOScale(1.1f, 0.2f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.DOKill();
        transform.DOScale(1f, 0.2f);
    }
}
