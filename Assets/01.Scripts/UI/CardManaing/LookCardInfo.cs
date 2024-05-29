using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class LookCardInfo : MonoBehaviour, IPointerMoveHandler, 
                                           IPointerExitHandler,
                                           IPointerClickHandler
{
    private InManigingCardInfoPanel _infoPanel;
    private Vector3 _savePoint;
    [SerializeField] private UnityEvent _cardVisualClickEvent;

    private void Start()
    {
        _infoPanel = GetComponentInChildren<InManigingCardInfoPanel>();
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        if (_infoPanel.IsActive()) return;

        if(_savePoint == Vector3.zero)
        {
            _savePoint = MaestrOffice.GetWorldPosToScreenPos(Input.mousePosition);
            return;
        }

        Vector3 mousePos = MaestrOffice.GetWorldPosToScreenPos(Input.mousePosition);
        bool isLeft = _savePoint.x > mousePos.x;

        _infoPanel.Active();
        _infoPanel.SetInfo(UIManager.Instance.GetSceneUI<CardManagingUI>().
                           SelectCardElement.CardInfo);
        _infoPanel.SetPosition(mousePos, isLeft);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _infoPanel.UnActive();
        _infoPanel.ClearInfo();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        _cardVisualClickEvent?.Invoke();
    }
}
