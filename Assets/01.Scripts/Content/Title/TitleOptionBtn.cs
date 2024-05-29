using System.Collections;
using System.Collections.Generic;
using UIDefine;
using UnityEngine;

public class TitleOptionBtn : TitleButton
{
    public override void PressEvent()
    {
        OptionPanel optionPanel =
            PanelManager.Instance.CreatePanel(PanelType.option, UIManager.Instance.CanvasTrm, Vector3.zero)
            as OptionPanel;

        optionPanel.PanelSetUp();
    }
}
