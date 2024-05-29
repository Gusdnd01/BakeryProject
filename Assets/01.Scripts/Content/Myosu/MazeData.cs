using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Myosu/Info")]
public class MazeData : ScriptableObject
{
    public int mazeLoad;
    public StageDataSO[] stageDataGroup = new StageDataSO[2];
}
