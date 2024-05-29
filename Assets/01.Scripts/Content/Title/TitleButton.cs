using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class TitleButton : MonoBehaviour
{
    [SerializeField] private Image _labelImg;

    [SerializeField] private float _startValue;
    [SerializeField] private float _endValue;

    private Tween _appearTween;
    private Tween _disappearTween;

    public void HobberEvent()
    {
        _disappearTween.Kill();

        _appearTween = DOTween.To(() => _startValue, sd => _labelImg.fillAmount = sd, _endValue, 0.1f);
    }
    public void UnHobberEvent()
    {
        _appearTween.Kill();

        _disappearTween = DOTween.To(() => _labelImg.fillAmount, sd => _labelImg.fillAmount = sd, _startValue, 0.1f);
    }
    public abstract void PressEvent();
}
