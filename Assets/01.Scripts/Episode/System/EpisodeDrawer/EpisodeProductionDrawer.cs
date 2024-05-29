using UnityEngine;
using System.Collections.Generic;
using EpisodeDialogueDefine;
using System;
using UnityEngine.UI;
using DG.Tweening;

public class EpisodeProductionDrawer : MonoBehaviour
{
    [Header("블랙 페널")]
    [SerializeField] private Image _blackPanel;
    [SerializeField] private RectTransform _blackPanelTrm;
    [SerializeField] private Color _blackColor;
    [SerializeField] private Color _alphaZeroBlackColor;

    [Header("수치 조정")]
    [SerializeField] private float _fadeInTime;
    [SerializeField] private float _fadeOutTime;

    [Header("에피소드 이미지 프리뷰")]
    [SerializeField] private GameObject _priviewObject;
    [SerializeField] private Image _priviewVisual;

    private event Action _fadeInOutAction;
    private FadeOutType _beforeFadeOutType;
    private Dictionary<FadeOutType, Action> _findFadeDic = new Dictionary<FadeOutType, Action>();

    public void HandleActivePriviewImage(bool isActive, Sprite visual)
    {
        if (_priviewObject.activeSelf == isActive && _priviewVisual.sprite == visual) return;

        _priviewObject.SetActive(isActive);
        _priviewVisual.sprite = visual;
    }

    private void OnDisable()
    {
        _findFadeDic.Clear();
    }

    private void OnEnable()
    {
        _findFadeDic = new Dictionary<FadeOutType, Action>();
        foreach(FadeOutType ft in Enum.GetValues(typeof(FadeOutType)))
        {
            switch (ft)
            {
                case FadeOutType.None:
                    _findFadeDic.Add(FadeOutType.None, HandleFadeOut);
                    break;
                case FadeOutType.Normal:
                    _findFadeDic.Add(FadeOutType.Normal, HandleFadeInNormal);
                    break;
                case FadeOutType.UpToDown:
                    _findFadeDic.Add(FadeOutType.UpToDown, HandleFadeInUpToDown);
                    break;
                case FadeOutType.LeftToRight:
                    _findFadeDic.Add(FadeOutType.LeftToRight, HandleFadeInLeftToRight);
                    break;
                default:
                    break;
            }
        }
    }

    public void HandleProductionDraw(FadeOutType fType)
    {
        if (fType == FadeOutType.None) return;

        _fadeInOutAction = null;

        _fadeInOutAction += _findFadeDic[fType];

        _fadeInOutAction?.Invoke();
        EpisodeManager.Instanace.ActiveSyntexPanel(false);
        _beforeFadeOutType = fType;
    }

    private void HandleFadeOut()
    {
        if (_beforeFadeOutType == FadeOutType.None) return;

        EpisodeManager.Instanace.NextDialogue();
        switch (_beforeFadeOutType)
        {
            case FadeOutType.Normal:
                Sequence _fadeSequence = DOTween.Sequence();
                _fadeSequence.Append(_blackPanel.DOFade(0, _fadeOutTime));
                _fadeSequence.AppendCallback(() =>
                {
                    _blackPanelTrm.localPosition = new Vector2(_blackPanelTrm.sizeDelta.x,
                                                               _blackPanelTrm.sizeDelta.y);
                    EpisodeManager.Instanace.ActiveSyntexPanel(true);
                });
                break;
            case FadeOutType.UpToDown:
                _blackPanelTrm.DOLocalMoveY(-_blackPanelTrm.sizeDelta.y, _fadeOutTime).OnComplete(() => EpisodeManager.Instanace.ActiveSyntexPanel(true));
                break;
            case FadeOutType.LeftToRight:
                 _blackPanelTrm.DOLocalMoveX(_blackPanelTrm.sizeDelta.x, _fadeOutTime).OnComplete(() => EpisodeManager.Instanace.ActiveSyntexPanel(true));
                break;
        }
        EpisodeManager.Instanace.ActiveSyntexPanel(true);
    }

    private void HandleFadeInNormal()
    {
        _blackPanel.color = _alphaZeroBlackColor;
        _blackPanelTrm.localPosition = Vector3.zero;

        _blackPanel.DOFade(1, _fadeInTime).OnComplete(HandleFadeOut);
    }

    private void HandleFadeInUpToDown()
    {
        _blackPanel.color = _blackColor;
        _blackPanelTrm.localPosition = new Vector2(0, _blackPanelTrm.sizeDelta.y);

        _blackPanelTrm.DOLocalMoveY(0, _fadeInTime).OnComplete(HandleFadeOut);

    }

    private void HandleFadeInLeftToRight()
    {
        _blackPanel.color = _blackColor;
        _blackPanelTrm.localPosition = new Vector2(-_blackPanelTrm.sizeDelta.x, 0);

        _blackPanelTrm.DOLocalMoveX(0, _fadeInTime).OnComplete(HandleFadeOut);
    }
}
