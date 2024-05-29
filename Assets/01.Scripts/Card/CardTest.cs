using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardTest : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private bool _onMouse;
    [SerializeField] private float _additionalValue;
    private Transform _visualTrm;

    public void OnPointerEnter(PointerEventData eventData)
    {
        _onMouse = true;
        _visualTrm = transform.Find("Visual");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _onMouse = false;
    }

    private void Update()
    {
        if (!_onMouse) return;

        Vector3 mouse = MaestrOffice.GetWorldPosToScreenPos(Input.mousePosition);
        Vector3 offset = transform.localPosition - mouse;

        float tiltX = offset.y * -1;
        float tiltY = offset.x;

        _visualTrm.localRotation = Quaternion.Euler(new Vector3(tiltX, tiltY, 0) * _additionalValue);
    }
}
