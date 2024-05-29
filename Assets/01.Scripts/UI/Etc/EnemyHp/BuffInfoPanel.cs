using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BuffInfoPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _skillNameText;
    [SerializeField] private TextMeshProUGUI _skillInfoText;

    public void SetInfo(string skillName, string skillInfo)
    {
        _skillNameText.text = skillName;
        _skillInfoText.text = skillInfo;
    }
}
