using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveBtn : ButtonElement
{
    public void SaveData(CanSaveData toSaveData, string saveKey, out bool isHasChange)
    {
        isHasChange = false;
        DataManager.Instance.SaveData(toSaveData, saveKey);
    }
}
