using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardInfoPanel : PoolableMono
{
    [SerializeField] private TextMeshProUGUI _skillNameText;
    [SerializeField] private TextMeshProUGUI _skillInfoText;

    public void SetInfo(CardInfo info, Transform parent)
    {
        _skillNameText.text = info.CardName;
        _skillInfoText.text = info.AbillityInfo;

        transform.SetParent(parent);
        transform.localPosition = Vector3.zero;
        transform.localScale = Vector3.one;
        transform.DOLocalMoveX(transform.localPosition.x + 550, 0.5f).SetEase(Ease.OutBack);
    }

    public void UnSetInfo()
    {
        transform.DOLocalMoveX(transform.localPosition.x - 550, 0.5f).SetEase(Ease.InBack).OnComplete(() => PoolManager.Instance.Push(this));
    }

    public override void Init()
    {

    }
}
