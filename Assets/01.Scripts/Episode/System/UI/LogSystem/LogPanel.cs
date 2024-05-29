using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogPanel : MonoBehaviour
{
    [SerializeField] private RectTransform _contentTrm;
    [SerializeField] private SyntexLog _syntexLogPrefab;

    public void ActivePanel(bool isActive)
    {
        gameObject.SetActive(isActive);
    }

    private void OnEnable()
    {
        foreach(LogData l in EpisodeManager.Instanace.dialogueLog)
        {
            _contentTrm.sizeDelta = new Vector2(_contentTrm.sizeDelta.x, _contentTrm.sizeDelta.y + 200);
            Instantiate(_syntexLogPrefab, _contentTrm).SetLog(l);
        }
    }
}
