using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ScrollFX : MonoBehaviour
{
    [SerializeField] private RectTransform leftCylinder;
    [SerializeField] private RectTransform rightCylinder;

    private RectTransform rt;

    private void Awake()
    {
        rt = GetComponent<RectTransform>();
    }

    private void ResetCylinder(bool isOpened)
    {
        leftCylinder.gameObject.SetActive(true);
        rightCylinder.gameObject.SetActive(true);

        leftCylinder.anchoredPosition = isOpened ? new Vector2(-900.0f, 0.0f) : new Vector2(0.0f,0.0f);
        rightCylinder.anchoredPosition = isOpened ? new Vector2(900.0f, 0.0f) : new Vector2(0.0f,0.0f);
    }

    private void MoveCylinder(float leftPos)
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(leftCylinder.DOAnchorPosX(leftPos, 1.0f));
        seq.Join(rightCylinder.DOAnchorPosX(-leftPos, 1.0f));
        seq.AppendCallback(() =>
        {
            leftCylinder.gameObject.SetActive(false);
            rightCylinder.gameObject.SetActive(false);
        });
    }

    [ContextMenu("Open")]
    public void OpenScroll()
    {
        ResetCylinder(false);
        MoveCylinder(-900.0f);
        rt.DOScaleX(1, 1.0f);
    }

    [ContextMenu("Close")]
    public void CloseScroll()
    {
        ResetCylinder(true);
        MoveCylinder(0.0f);
        rt.DOScaleX(0, 1.0f);
    } 
}
