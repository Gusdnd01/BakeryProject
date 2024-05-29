using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonElement : MonoBehaviour
{
    private Button _myBtn;
    private Button MyBtn
    {
        get
        {
            if(_myBtn != null) return _myBtn;

            _myBtn = GetComponent<Button>();
            return _myBtn;
        }
    }

    public void SetOneShotCallbackToPress(UnityAction callBack)
    {
        MyBtn.onClick.AddListener(callBack);
    }

    public void SetCallbackToPress(UnityAction callBack)
    {
        MyBtn.onClick.RemoveAllListeners();
        MyBtn.onClick.AddListener(callBack);
    }
}
