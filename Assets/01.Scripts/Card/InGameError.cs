using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameError : MonoBehaviour
{
    [SerializeField] private Transform _errorTextTrm;

    public void ErrorSituation(string errorText)
    {
        ErrorText et = PoolManager.Instance.Pop(PoolingType.ErrorText) as ErrorText;
        et.transform.SetParent(_errorTextTrm);

        et.Erroring(errorText);
    }
}
