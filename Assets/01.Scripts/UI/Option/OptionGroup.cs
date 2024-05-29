using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionGroup : MonoBehaviour
{
    private InfoBlockSelectBtn[] _infoBlockArr;
    public SaveBtn saveBtn;
    public SetInitialValueBtn setInitialBtn;

    private void Awake()
    {
        _infoBlockArr = FindObjectsOfType<InfoBlockSelectBtn>();
    }

    public void ManagingActivationInfoBlock(InfoBlockSelectBtn infoSelectBtn)
    {
        foreach(InfoBlockSelectBtn ibsb in _infoBlockArr)
        {
            if(infoSelectBtn == ibsb)
            {
                if (infoSelectBtn.ClickThisPanel) break;

                infoSelectBtn.ClickThisPanel = true;
                continue;
            }
            ibsb.ClickThisPanel = false;
        }
    }
}
