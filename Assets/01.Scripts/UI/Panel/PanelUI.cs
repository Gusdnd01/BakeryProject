using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
using UIDefine;

[Serializable]
public class PanelUI : MonoBehaviour
{
    [SerializeField] private PanelType _myPanelType;
    public PanelType PanelType => _myPanelType;

    #region BlackPanelFamily
    public bool useBlackPanel = false;
    [HideInInspector] public Image blackPanel;
    [HideInInspector] public float easingTime = 0.2f;
    [HideInInspector] public float endOfAlpha = 0.3f;
    #endregion

    public bool canSeconderyActivePanel;
    [HideInInspector] public int toCreatedCount = 3;

    public void ActivePanel(bool isActive)
    {
        if(useBlackPanel)
        {
            Color bpColor = blackPanel.color;
            bpColor.a = endOfAlpha;
            blackPanel.color = bpColor;
        }
        gameObject.SetActive(isActive);
    }

    public void FadePanel(bool isActive)
    {
        if(!useBlackPanel)
        {
            Debug.LogError("Error! This panel not use 'BlackPanel'!!\nIf you want to use 'BlackPanel', you must check 'useBlackPanel'(type : Boolean)");
            return;
        }

        blackPanel.DOFade(endOfAlpha * MaestrOffice.BoolToInt(isActive), easingTime).
        OnComplete(() => blackPanel.raycastTarget = isActive);
    }

    public void FadePanel(bool isActive, Action callBack)
    {
        if (!useBlackPanel)
        {
            Debug.LogError("Error! This panel not use 'BlackPanel'!!\nIf you want to use 'BlackPanel', you must check 'useBlackPanel'(type : Boolean)");
            return;
        }

        blackPanel.DOFade(endOfAlpha * MaestrOffice.BoolToInt(isActive), easingTime).OnComplete(()=> callBack());
    }
}
