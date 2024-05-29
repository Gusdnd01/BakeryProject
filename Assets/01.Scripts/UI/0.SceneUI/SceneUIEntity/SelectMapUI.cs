using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectMapUI : SceneUI
{
    [SerializeField] private MapSelectPanelLock[] _panelLockArr;
    private AdventureData _adventureData = new AdventureData();

    public override void SceneUIStart()
    {
        SceneObserver.BeforeSceneType = SceneType.Lobby;

        GenerateUnLockPanel();
    }

    public AdventureData GetAdventureData()
    {
        return DataManager.Instance.LoadData<AdventureData>(DataKeyList.adventureDataKey);
    }

    private void GenerateUnLockPanel()
    {
        if (DataManager.Instance.IsHaveData(DataKeyList.adventureDataKey))
        {
            _adventureData = DataManager.Instance.LoadData<AdventureData>(DataKeyList.adventureDataKey);
        }

        int chapterIdx = Convert.ToInt16(_adventureData.InChallingingStageCount.Split('-')[0]);
        for (int i = 0; i < chapterIdx; i++)
        {
            if (_adventureData.IsLookUnLockProductionArr[i])
            {
                _panelLockArr[i].UnLockStageWithOutProduction();
            }
            else
            {
                _panelLockArr[i].UnLockStageWithProduction();
                _adventureData.IsLookUnLockProductionArr[i] = true;
            }
        }

        DataManager.Instance.SaveData(_adventureData, DataKeyList.adventureDataKey);
    }
}
