using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[System.Serializable]
public class OptionPanel : PanelUI
{

    [Header("옵션패널")]
    [SerializeField] private Transform _optionPanelGroup;
    [SerializeField] private float _optionPanelActivePosY;

    public void PanelSetUp()
    {
        _optionPanelGroup.transform.localPosition = new Vector3(0, _optionPanelActivePosY, 0);
        blackPanel.color = new Color(0, 0, 0, 0);

        gameObject.SetActive(true);
        FadePanel(true);
        _optionPanelGroup.transform.DOLocalMoveY(0, easingTime).SetEase(Ease.OutQuart);
    }

    public void PanelInit()
    {
        _optionPanelGroup.transform.DOLocalMoveY(_optionPanelActivePosY, easingTime).SetEase(Ease.InBack);
        FadePanel(false, () => PanelManager.Instance.DeletePanel(this));
    }
}
