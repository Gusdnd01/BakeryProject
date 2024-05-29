using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UIDefine;
using UnityEngine.InputSystem;

public class AlwaysActiveOption : MonoBehaviour
{
    private void Update()
    {
        if (GameManager.Instance.GetCurrentSceneInfo().buildIndex == 1) return;

        if(Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            OptionPanel optionPanel =
            PanelManager.Instance.CreatePanel(PanelType.option, UIManager.Instance.CanvasTrm, Vector3.zero)
            as OptionPanel;

            optionPanel.PanelSetUp();
        }
    }
}
