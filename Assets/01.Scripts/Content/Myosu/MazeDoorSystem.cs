using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MazeDoorSystem : MonoBehaviour
{
    private MazeContainer _mazeContainer;
    [SerializeField] private Image _wallImg;
    private MazeDoor[] _mazeDoorArr;
    private const string _dataKey = "AdventureKEY";

    private void Awake()
    {
        _mazeContainer = GetComponent<MazeContainer>();
        _mazeDoorArr = GetComponentsInChildren<MazeDoor>();

        AdventureData data = DataManager.Instance.LoadData<AdventureData>(_dataKey);
        int load = Convert.ToInt16(data.InChallingingMazeLoad);

        StageDataSO[] sdArr = _mazeContainer.GetMazeDataByLoad(load);

        for(int i = 0; i < sdArr.Length; i++)
        {
            _mazeDoorArr[i].AssignedStageInfo = sdArr[i];
        }
    }

    public void SelectDoor(MazeDoor mazeDoor)
    {
        foreach (MazeDoor md in _mazeDoorArr)
        {
            md.CanInteractible = false;
            if (md != mazeDoor)
            {
                md.Visual.DOFade(0, 0.2f);
            }
        }
    }    
    public void HoverDoor(MazeDoor mazeDoor)
    {
        _wallImg.DOFade(0.8f, 0.2f);
        foreach(MazeDoor md in _mazeDoorArr)
        {
            if (md != mazeDoor)
            {
                md.Visual.DOFade(0.2f, 0.3f);
            }
        }
    }
    public void UnHoverDoor(MazeDoor mazeDoor)
    {
        _wallImg.DOFade(0.5f, 0.2f);
        foreach (MazeDoor md in _mazeDoorArr)
        {
            if (md != mazeDoor)
            {
                md.Visual.DOFade(1f, 0.3f);
            }
        }
    }
}
