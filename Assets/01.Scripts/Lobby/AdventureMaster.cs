using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public struct MissionPanel
{
    public TextMeshProUGUI clearCountTxt;
}

[Serializable]
public struct MinePanel
{
    public TextMeshProUGUI clearCountTxt;
}

[Serializable]
public struct StagePanel
{
    public TextMeshProUGUI inStageCount;
    public List<Sprite> chapterVisualList;
    public Image visual;
}

public class AdventureMaster : MonoBehaviour
{
    private AdventureData _adventureData = new AdventureData();
    private const string _adventureKey = "AdventureKEY";

    [SerializeField] private MissionPanel _missionPanel = new MissionPanel();
    [SerializeField] private MinePanel _minePanel = new MinePanel();
    [SerializeField] private StagePanel _stagePanel = new StagePanel();

    private void Start()
    {
        if (DataManager.Instance.IsHaveData(_adventureKey))
        {
            _adventureData = DataManager.Instance.LoadData<AdventureData>(_adventureKey);
        }

        _missionPanel.clearCountTxt.text = $"<size=40>탐색 중인 향로</size> : {_adventureData.InChallingingMazeLoad}";
        _minePanel.clearCountTxt.text = $"정복 중인 층 : {_adventureData.ChallingingMineFloor}";
        _stagePanel.inStageCount.text = $"도전 중인 지역 : {_adventureData.InChallingingStageCount}";

        int idx = Convert.ToInt16(_adventureData.InChallingingStageCount.Split('-')[0]);
        _stagePanel.visual.sprite = _stagePanel.chapterVisualList[idx - 1];
    }
}
