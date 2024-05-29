using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class TeaTimeCakeObject : MonoBehaviour, IDragHandler, IEndDragHandler
{
    public bool CanCollocate { get; set; } = true;
    [SerializeField] private Image _cakeImg;
    [SerializeField] private RectTransform _rectTrm;
    private Vector2 _usuallyPos;

    private ItemDataBreadSO _cakeSO;
    public ItemDataBreadSO CakeInfo => _cakeSO;

    private TeaTimeUI _teaTimeUI;
    private bool _isInitThisCakeInEatRange;

    private void Start()
    {
        _teaTimeUI = UIManager.Instance.GetSceneUI<TeaTimeUI>();
        _usuallyPos = transform.position;
    }

    public void SetCakeImage(ItemDataBreadSO info)
    {
        _cakeSO = info;
        _cakeImg.sprite = info.itemIcon;
        _cakeImg.enabled = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = MaestrOffice.GetWorldPosToScreenPos(Input.mousePosition);
        _isInitThisCakeInEatRange = _teaTimeUI.EatRange.CheckCanEat(_rectTrm);
        _teaTimeUI.TeaTimeCreamStand.ChangeFace(_isInitThisCakeInEatRange);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if(_isInitThisCakeInEatRange)
        {
            _teaTimeUI.TeaTimeCreamStand.EatCake(CakeInfo);
            _teaTimeUI.CakeCollocation.UnCollocateCake(CakeInfo);
            _teaTimeUI.TeaTimeCreamStand.Reaction();
            UIManager.Instance.GetSceneUI<TeaTimeUI>().SetCard(CakeInfo.ToGetCardBase.CardInfo);
            _cakeImg.enabled = false;
        }
        else
        {
            _cakeImg.enabled = true;
        }
        transform.position = _usuallyPos;
    }
}
