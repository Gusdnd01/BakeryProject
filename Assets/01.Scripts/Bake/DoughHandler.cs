using DG.Tweening;
using ExtensionFunction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DoughHandler : MonoBehaviour
{
    [Header("StoveRange")]
    [SerializeField] private Vector2 _stoveMaxRange;
    [SerializeField] private Vector2 _stoveMinRange;
    [SerializeField] private Vector2 _stoveEnterPos;

    [Space(10)]

    private Vector2 _doughNormalPos;
    [SerializeField] private bool _isInnerDough;
    private bool _isInRange;
    private bool _IsInRange
    {
        get
        {
            return _isInRange;
        }
        set
        {
            if (_isInRange == value) return;

            if(value)
            {
                _doughEnterRangeEvent?.Invoke();
            }
            else
            {
                _doughExitRangeEvent?.Invoke();
            }

            _isInRange = value;
        }
    }

    [SerializeField] private UnityEvent _doughEnterRangeEvent;
    [SerializeField] private UnityEvent _doughExitRangeEvent;
    [SerializeField] private UnityEvent _doughToInnerEndEvent;

    void Start()
    {
        _doughNormalPos = transform.position;
    }
    private void OnMouseEnter()
    {
        if (Input.GetMouseButton(0)) return;

        _isInnerDough = true;
    }
    private void OnMouseExit()
    {
        if (Input.GetMouseButton(0)) return;

        _isInnerDough = false;
    }
    void Update()
    {
        ActiveCheck();
    }
    private void ActiveCheck()
    {
        if (Input.GetMouseButton(0) && _isInnerDough)
        {
            Vector2 mousePos = MaestrOffice.GetWorldPosToScreenPos(Input.mousePosition);
            Vector2 myPos = Vector2.Lerp(transform.position, mousePos, Time.deltaTime * 20f);
            transform.position = myPos;

            if (myPos.x > _stoveMaxRange.x && myPos.y < _stoveMaxRange.y &&
                myPos.x < _stoveMinRange.x && myPos.y > _stoveMinRange.y)
            {
                _IsInRange = true;
            }
            else
            {
                _IsInRange = false;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            _isInnerDough = false;
            transform.position = transform.position;
            ActiveInnerStoveRange();
        }
    }
    private void ActiveInnerStoveRange()
    {
        if(_IsInRange)
        {
            _doughToInnerEndEvent?.Invoke();
            transform.SmartScale(Vector2.one * 0.5f, 0.1f);
        }
        else
        {
            _isInnerDough = false;
            transform.position = _doughNormalPos;
        }
    }    
}
