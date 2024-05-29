using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionBtn : MonoBehaviour
{
    private bool _isPanelHide = false;

    private void Update()
    {
        if(Input.GetMouseButtonDown(1))
        {
            EpisodeManager.Instanace.ActiveSyntexPanel(_isPanelHide);
            _isPanelHide = !_isPanelHide;
        }
    }
}
