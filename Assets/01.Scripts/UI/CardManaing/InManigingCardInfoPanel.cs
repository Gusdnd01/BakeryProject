using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InManigingCardInfoPanel : MonoBehaviour
{
    [SerializeField] private CanvasGroup _infoVisualGroup;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _infoText;

    [Header("ÇÇº¿ ÁÂÇ¥")]
    [SerializeField] private Vector2 _leftPivot;
    [SerializeField] private Vector2 _rightPivot;

    private RectTransform _rectTrm;
    private Tween _alphaTween;
    private Tween _scalingTween;

    private void Awake()
    {
        _rectTrm = transform as RectTransform;
    }

    public bool IsActive()
    {
        return _infoVisualGroup.alpha > 0;
    }

    public void SetPosition(Vector2 mousePoint, bool isLeft)
    {
        _rectTrm.pivot = isLeft ? _leftPivot : _rightPivot;
        transform.position = mousePoint;
    }

    public void SetInfo(CardInfo info)
    {
        _nameText.text = info.CardName;
        _infoText.text = info.AbillityInfo;
    }

    public void ClearInfo()
    {
        _nameText.text = string.Empty;
        _infoText.text = string.Empty;
    }

    public void Active()
    {
        _alphaTween.Kill();
        _scalingTween.Kill();

        _rectTrm.localScale = Vector3.one * 0.8f;
        _alphaTween = _infoVisualGroup.DOFade(1, 0.1f);
        _scalingTween = _rectTrm.DOScale(1, 0.1f);
    }

    public void UnActive()
    {
        _alphaTween.Kill();
        _scalingTween.Kill();

        _rectTrm.localScale = Vector3.one * 0.8f;
        _alphaTween = _infoVisualGroup.DOFade(0, 0.1f);
        _scalingTween = _rectTrm.DOScale(0.8f, 0.1f);
    }
}
