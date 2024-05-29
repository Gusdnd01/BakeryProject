using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveSelectCakePanel : MonoBehaviour
{
    [SerializeField] private CakeInventoryPanel _cakePanel;

    public void ActiveCakeInventoryPanel(bool isActive)
    {
        _cakePanel.gameObject.SetActive(isActive);
        _cakePanel.FadePanel(isActive);
    }
}
