using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageBubble : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI _stageNameText;
    [SerializeField] private Transform _infoBlockTrm;

    public void SetInfo(string stageName, bool isReverse)
    {
        _stageNameText.text = stageName;

        if(isReverse)
        {
            transform.rotation = Quaternion.Euler(0, 0, -180);
            _infoBlockTrm.localRotation = Quaternion.Euler(0, 0, -180);
        }
    }

    public void EnterStage()
    {
        GameManager.Instance.ChangeScene(SceneType.battle);
    }
}
