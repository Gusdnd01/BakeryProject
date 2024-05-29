using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleResultAccept : MonoBehaviour
{
    [SerializeField] private Button _myButton;
    [SerializeField] private GameObject _battleResultPanel;

    private void OnEnable()
    {
        if(MapManager.Instanace.SelectStageData.stageType != StageType.Mine)
        {
            _myButton.onClick.AddListener(StageAccept);
        }
        else
        {
            _myButton.onClick.AddListener(MineAccept);
        }
    }

    private void OnDisable()
    {
        _myButton.onClick.RemoveAllListeners();
    }

    public void StageAccept()
    {
        GameManager.Instance.ChangeScene(SceneObserver.BeforeSceneType);
    }

    public void MineAccept()
    {
        UIManager.Instance.GetSceneUI<MineUI>().MineSystem.CurrentMineInfo.IsClearThisStage = true;
        GameManager.Instance.ChangeScene(SceneObserver.CurrentSceneType);
    }
}
