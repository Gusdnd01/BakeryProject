using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualMinus : MonoBehaviour
{
    [SerializeField] private SpriteRenderer ren;
    void Start()
    {
        GetComponent<SpriteRenderer>().sortingOrder = ren.sortingOrder-1;
    }
}
