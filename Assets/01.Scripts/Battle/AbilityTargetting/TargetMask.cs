using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TargetMask : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image _targetMarkImg;
    private Tween _fadeTween;

    private void ActiveTargetMark(bool isActive)
    {
        _fadeTween.Kill();
        _fadeTween = _targetMarkImg.DOFade(MaestrOffice.BoolToInt(isActive), 0.2f);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!CardReader.AbilityTargetSystem.OnTargetting) return;

        CardReader.AbilityTargetSystem.CanBinding = false;
        CardReader.AbilityTargetSystem.mousePos = transform.localPosition;
        ActiveTargetMark(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!CardReader.AbilityTargetSystem.OnTargetting) return;

        CardReader.AbilityTargetSystem.CanBinding = true;
        ActiveTargetMark(false);
    }
}
