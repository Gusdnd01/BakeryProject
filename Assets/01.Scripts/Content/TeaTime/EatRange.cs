using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EatRange : MonoBehaviour
{
    [SerializeField] private Vector2 _upperPos;
    [SerializeField] private Vector2 _dwonerPos;

    public bool CheckCanEat(RectTransform cakeRect)
    {
        Vector2 checkRectPos = cakeRect.localPosition;

        return (_upperPos.x < checkRectPos.x && _upperPos.y > checkRectPos.y) &&
               (_dwonerPos.x > checkRectPos.x && _dwonerPos.y < checkRectPos.y);
    }
}
