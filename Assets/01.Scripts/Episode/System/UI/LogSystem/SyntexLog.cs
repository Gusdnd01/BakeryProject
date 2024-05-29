using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SyntexLog : MonoBehaviour
{
    [SerializeField] private Image _profile;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _syntexText;

    public void SetLog(LogData logData)
    {
        _profile.sprite = logData.characterSprite;
        _nameText.text = logData.characterName;
        _syntexText.text = logData.characterSyntex;
    }
}
