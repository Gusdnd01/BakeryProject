using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineSystem : MonoBehaviour
{
    [Header("¸Ê")]
    [SerializeField] private Transform _firstMap;
    [SerializeField] private Transform _secondMap;
    [SerializeField] private Vector3 _downPos;

    [SerializeField] private MineInfoContainer _mineContainer;
    public MineInfo CurrentMineInfo { get; private set; }
    private AdventureData _addData = new AdventureData();

    private void Start()
    {
        if(DataManager.Instance.IsHaveData(DataKeyList.adventureDataKey))  
        {
            _addData = DataManager.Instance.LoadData<AdventureData>(DataKeyList.adventureDataKey);
        }

        CurrentMineInfo = _mineContainer.GetInfoByFloor(Convert.ToInt16(_addData.ChallingingMineFloor));
        Debug.Log(CurrentMineInfo);
        MapManager.Instanace.SelectStageData = CurrentMineInfo.stageData;
        MineUI mineUI = UIManager.Instance.GetSceneUI<MineUI>();

        if(!CurrentMineInfo.IsClearThisStage)
        {
            mineUI.SetFloor(CurrentMineInfo.Floor.ToString(),
                        CurrentMineInfo.stageData.stageName,
                        CurrentMineInfo.ClearGem,
                        CurrentMineInfo.IsClearThisStage);
            mineUI.SetUpFloor();

            mineUI.StagePanelAnimator.enabled = true;
        }
        else
        {
            ClearStage();
        }
    }

    public void ClearStage()
    {
        CurrentMineInfo.IsClearThisStage = true;
        int uf = CurrentMineInfo.Floor + 1;
        
        CurrentMineInfo = _mineContainer.GetInfoByFloor(uf);

        _addData.ChallingingMineFloor = CurrentMineInfo.Floor.ToString();
        DataManager.Instance.SaveData(_addData, DataKeyList.adventureDataKey);

        MineUI mineUI = UIManager.Instance.GetSceneUI<MineUI>();
        Debug.Log(CurrentMineInfo);

        mineUI.SetFloor(CurrentMineInfo.Floor.ToString(), 
                        CurrentMineInfo.stageData.stageName, 
                        CurrentMineInfo.ClearGem, 
                        CurrentMineInfo.IsClearThisStage);

        mineUI.SetUpFloor();
        MapChange();
    }

    private void MapChange()
    {
        Transform upTrm;
        Transform downTrm;

        if(_firstMap.position.y > _secondMap.position.y)
        {
            upTrm = _firstMap;
            downTrm = _secondMap;
        }
        else
        {
            upTrm = _secondMap;
            downTrm = _firstMap;
        }
        
        upTrm.transform.position = _downPos;
        upTrm.gameObject.SetActive(true);

        _firstMap.DOMoveY(_firstMap.position.y + 14, 2f);
        _secondMap.DOMoveY(_secondMap.position.y + 14, 2f).OnComplete(()=> 
        {
            upTrm.GetComponentInChildren<MineTape>().LockTape(false);
            downTrm.GetComponentInChildren<MineTape>().LockTape(true);
            downTrm.gameObject.SetActive(false);
            UIManager.Instance.GetSceneUI<MineUI>().StagePanelAnimator.enabled = true;
            UIManager.Instance.GetSceneUI<MineUI>().PanelActive(true);
        });
    }
}
