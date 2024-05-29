using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdventureData : CanSaveData
{
    public string ChallingingMineFloor = "1";
    public bool[] IsLookUnLockProductionArr = new bool[6];
    public string InChallingingMazeLoad = "1";
    public string InChallingingStageCount = "1-1";

    public override void SetInitialValue()
    {

    }
}
