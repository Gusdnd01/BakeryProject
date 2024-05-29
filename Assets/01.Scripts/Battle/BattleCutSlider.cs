using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleCutSlider : MonoBehaviour
{
    private Sequence cuttingSeq;
    [SerializeField] private float _normalValue;
    [SerializeField] private float _cuttingValue;

    [SerializeField] private Transform _ceilCutSlider;
    [SerializeField] private Transform _bottomCutSlider;

    public void Cutting()
    {
        SetUp();

        cuttingSeq.Append(_ceilCutSlider.DOLocalMoveY(_normalValue - _cuttingValue, 0.2f).SetEase(Ease.OutCubic));
        cuttingSeq.Join(_bottomCutSlider.DOLocalMoveY(-_normalValue + _cuttingValue, 0.2f).SetEase(Ease.OutCubic));
    }

    public void Reverting()
    {
        SetUp();

        cuttingSeq.Append(_ceilCutSlider.DOLocalMoveY(_normalValue, 0.2f).SetEase(Ease.InCubic));
        cuttingSeq.Join(_bottomCutSlider.DOLocalMoveY(-_normalValue, 0.2f).SetEase(Ease.InCubic));
    }

    private void SetUp()
    {
        cuttingSeq?.Kill();
        cuttingSeq = DOTween.Sequence();
    }
}
