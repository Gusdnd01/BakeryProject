using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeContainer : MonoBehaviour
{
    [SerializeField] private MazeData[] _mazeDataArr;
    private Dictionary<int, MazeData> _getMazeDataDic = new ();

    public StageDataSO[] GetMazeDataByLoad(int load)
    {
        if(_getMazeDataDic.Count != _mazeDataArr.Length)
        {
            Debug.Log("MazeDataRead");
            _getMazeDataDic.Clear();

            foreach (MazeData data in _mazeDataArr)
            {
                _getMazeDataDic.Add(data.mazeLoad, data);
            }
        }

        return _getMazeDataDic[load].stageDataGroup;
    }
}
