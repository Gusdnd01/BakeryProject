using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class ErrorText : PoolableMono
{
    [SerializeField] private Image _errorImg;
    [SerializeField] private TextMeshProUGUI _errorText;

    [SerializeField] private float _fadeTime;

    public void Erroring(string errorText)
    {
        transform.localScale = Vector3.one;
        transform.localPosition = Vector3.zero;

        _errorText.text = errorText;
        transform.DOLocalMoveY(transform.localPosition.y + 30f, _fadeTime - 0.2f);
        _errorText.DOFade(0, _fadeTime);
        _errorImg.DOFade(0, _fadeTime).OnComplete(() => PoolManager.Instance.Push(this));
    }

    public override void Init()
    {
        _errorText.color = Color.white;
        _errorImg.color = Color.white;
    }
}
