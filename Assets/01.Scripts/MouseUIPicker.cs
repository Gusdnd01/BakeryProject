using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseUIPicker : MonoBehaviour
{
    [SerializeField] private bool _canPick;

    private void Update()
    {
        if(Input.GetMouseButtonDown(0) && _canPick)
        {
            CheckUI();
        }
    }

    private void CheckUI()
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, results);

        foreach (RaycastResult result in results)
        {
            Debug.Log($"{result.gameObject.name}, Parent : {result.gameObject.transform.parent.gameObject.name}" );
        }
    }
}
