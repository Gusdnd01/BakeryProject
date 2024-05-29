using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetInitialValueBtn : ButtonElement
{
    public void InitializeData(CanSaveData toSaveData, out bool isHasChange)
    {
        isHasChange = false;
        toSaveData.SetInitialValue();
    }
}
