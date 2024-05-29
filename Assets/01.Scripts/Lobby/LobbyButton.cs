using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.InputSystem;
using UIDefine;
using UnityEngine.EventSystems;

public class LobbyButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private SceneType _toGoScene;
    [SerializeField] private Transform _visualTrm;
    private Tween _hoverTween;

    public void OnPointerEnter(PointerEventData eventData)
    {
        _hoverTween.Kill();
        _hoverTween = _visualTrm.DOScale(Vector2.one * 1.1f, 0.1f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _hoverTween.Kill();
        _hoverTween = _visualTrm.DOScale(Vector2.one, 0.1f);
    }

    private void OnDisable()
    {
        _hoverTween.Kill();
    }

    public void PressThisButton()
    {
        GameManager.Instance.ChangeScene(_toGoScene);
    }
}
                                             