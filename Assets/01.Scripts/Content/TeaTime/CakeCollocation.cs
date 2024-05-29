using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CakeCollocation : MonoBehaviour
{
    [SerializeField] private TeaTimeCakeObject[] _cakeObjectArr = new TeaTimeCakeObject[3];

    public void UnCollocateCake(ItemDataBreadSO cakeInfo)
    {
        for (int i = 0; i < _cakeObjectArr.Length; i++)
        {
            if (_cakeObjectArr[i].CakeInfo == cakeInfo)
            {
                _cakeObjectArr[i].CanCollocate = true;
                return;
            }
        }

        Debug.LogWarning("케이크 없음");
    }

    public void CollocateCake(ItemDataBreadSO cakeInfo)
    {
        for (int i = 0; i < _cakeObjectArr.Length; i++)
        {
            if (_cakeObjectArr[i].CanCollocate)
            {
                _cakeObjectArr[i].SetCakeImage(cakeInfo);
                _cakeObjectArr[i].CanCollocate = false;
                return;
            }
        }

        Debug.LogWarning("자리 없음");
    }
}
