using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapSelectPanelLock : MonoBehaviour
{
    [SerializeField] private Image _lockPanel;
    [SerializeField] private Image _lockIcon;
    [SerializeField] private GameObject _doNotEnterTxt;

    private void Awake()
    {
        _lockPanel = GetComponent<Image>();
        _lockIcon = transform.Find("Lock").GetComponent<Image>();
        _doNotEnterTxt = transform.Find("LockText").gameObject;
    }

    public void UnLockStageWithProduction()
    {
        _doNotEnterTxt.SetActive(false);

        Sequence panelUnLockSeq = DOTween.Sequence();
        panelUnLockSeq.Append(_lockIcon.transform.DOShakePosition(1, 10, 30));
        panelUnLockSeq.Append(_lockPanel.DOFade(0, 0.4f));
        panelUnLockSeq.Join(_lockIcon.DOFade(0, 0.4f));
        panelUnLockSeq.AppendCallback(()=> _lockPanel.gameObject.SetActive(false));
    }

    public void UnLockStageWithOutProduction()
    {
        _lockPanel.gameObject.SetActive(false);
    }
}
