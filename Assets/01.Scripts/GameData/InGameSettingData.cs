using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameSettingData : CanSaveData
{
    public float vibrationValue;
    public int modeNum;
    public bool isVerticalSync;

    public override void SetInitialValue()
    {
        vibrationValue = 100;
        modeNum = 1;
        isVerticalSync = true;
    }
}
