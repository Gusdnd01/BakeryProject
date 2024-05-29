using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDrawer : MonoBehaviour
{
    private int idx = 0;
    [SerializeField]private List<CardBase> l;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            idx = (idx + 1) % l.Count;
            print(l[idx]);
        }
        if (Input.GetKeyDown(KeyCode.Space))
            CardReader.CardDrawer.TestDraw(l[idx]);
    }
}
