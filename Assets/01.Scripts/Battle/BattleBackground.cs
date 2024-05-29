using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleBackground : MonoBehaviour
{
    [SerializeField] private GameObject[] _backGroundArr;

    public void SetBG()
    {
        for(int i = 0; i < _backGroundArr.Length; i++)
        {
            _backGroundArr[i].SetActive((int)MapManager.Instanace.SelectStageData.stageBackGround == i);
        }
    }
}
