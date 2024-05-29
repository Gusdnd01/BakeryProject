using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class InputBuffer
{
    private float _timestamp;
    private PlayerInput _inputType;
    private bool _trigger;
    public bool Trigger
    {
        get
        {
            bool b = false;
            if(_trigger)
            {
                b = true;
                _trigger = false;
            }
            return b;
        }
    }

    public static float TimeBeforeActionsExpire = 0.05f;

    public void GetInput(float stamp, PlayerInput inputType)
    {
        _timestamp = stamp;
        _inputType = inputType;
        _trigger = true;
    }

    //returns true if this action hasn't expired due to the timestamp
    public bool CheckIfValid(out PlayerInput inputType)
    {
        inputType = _inputType;
        return _trigger && Time.time - _timestamp < TimeBeforeActionsExpire;
    }

}
