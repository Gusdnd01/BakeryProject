using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SyntexPanel : MonoBehaviour
{
    [SerializeField] private Image[] _panels;
    [SerializeField] private TextMeshProUGUI[] _texts;

    public void NextDialogue()
    {
        EpisodeManager.Instanace.NextDialogue();
    }

    public void HidePnale(bool isActive)
    {
        foreach(Image p in _panels)
        {
            p.enabled = isActive;
        }
        foreach(TextMeshProUGUI t in _texts)
        {
            t.enabled = isActive;
        }
    }
}
