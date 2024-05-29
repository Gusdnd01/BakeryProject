using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuffingMark : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public CombatMarkingData CombatMarkingData { get; set; }

    [SerializeField] private Image _visual;
    [SerializeField] private BuffInfoPanel _infoPanelPrefab;
    [SerializeField] private RectTransform _infoPanelTrm;
    private BuffInfoPanel _currentInfoPanel;

    public int VisualIndex { get; private set; }
    private Transform _infoPanelParent;
    private string _buffName;
    private string _buffInfo;

    private Tween _infoPanelTween;

    public void SetInfo(Sprite visual, string buffName, CombatMarkingData data, Transform panelTrm)
    {
        CombatMarkingData = data;

        _buffName = buffName;
        _visual.sprite = visual;
        _buffInfo = data.buffingInfo;
        _infoPanelParent = panelTrm;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (CardReader.AbilityTargetSystem.OnTargetting) return;
        _infoPanelTween.Kill();

        if(_currentInfoPanel != null)
        {
            Destroy(_currentInfoPanel.gameObject);
        }

        _currentInfoPanel = Instantiate(_infoPanelPrefab, _infoPanelParent);
        Debug.Log(_currentInfoPanel.transform.parent);
        _currentInfoPanel.SetInfo(_buffName, _buffInfo);
        _currentInfoPanel.transform.position = _infoPanelTrm.position;
        
        _infoPanelTween = _currentInfoPanel.transform.DOScaleX(1, 0.1f).SetEase(Ease.OutBounce);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (CardReader.AbilityTargetSystem.OnTargetting) return;

        Debug.Log(_currentInfoPanel);
        _infoPanelTween.Kill();
        _infoPanelTween = _currentInfoPanel.transform.DOScaleX(0, 0.1f).SetEase(Ease.InBounce).
                          OnComplete(() => Destroy(_currentInfoPanel.gameObject));
    }
}
