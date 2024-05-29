using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public struct NormalPanelInfo
{
    public string subject;
    public string content;
    public bool useBlackPanel;

    public NormalPanelInfo(string _subject, string _content, bool _useBlackPanel)
    {
        subject = _subject;
        content = _content;
        useBlackPanel = _useBlackPanel;
    }
}

public class NormalPanel : PanelUI
{
    [SerializeField] private TextMeshProUGUI _subjectText;
    [SerializeField] private TextMeshProUGUI _contentText;

    public void SetUpPanel(NormalPanelInfo info)
    {
        _subjectText.text = info.subject;
        _contentText.text = info.content;
        if(info.useBlackPanel)
        {
            FadePanel(true);
        }
    }
}
