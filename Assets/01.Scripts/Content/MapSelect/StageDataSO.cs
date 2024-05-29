using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StageType
{
    Main,
    Mine,
    Mission
}

public enum StageBackGround
{
    Forest,
    Dungeon,
    Myosu,
    Desert,
    Winter
}

[Serializable]
public class Compensation
{
    public ItemDataSO Item;
    public int count;
}

[CreateAssetMenu(menuName ="SO/StageData")]
public class StageDataSO : ScriptableObject
{
    public string stageNumber;
    public string stageName;
    public StageType stageType;
    public StageBackGround stageBackGround;
    public EnemyGroupSO enemyGroup;
    public TsumegoInfo clearCondition;
    public Compensation compensation;
    public bool isClearThisStage;

    public void Clone()
    {
        clearCondition = Instantiate(clearCondition);
    }

    public void StageClear()
    {
        if (isClearThisStage) return;

        isClearThisStage = true;
        string[] numArr = stageNumber.Split('-');

        int chapteridx = Convert.ToInt16(numArr[0]);
        int stageidx = Convert.ToInt16(numArr[1]);

        AdventureData ad = DataManager.Instance.LoadData<AdventureData>(DataKeyList.adventureDataKey);
        Debug.Log($"{chapteridx}-{stageidx}");
        string challingingStageData = $"{chapteridx}-{stageidx + 1}";
        if(stageidx == 6)
        {
            challingingStageData = $"{chapteridx + 1}-{1}";
        }
        Debug.Log(challingingStageData);
        ad.InChallingingStageCount = challingingStageData;
        DataManager.Instance.SaveData(ad, DataKeyList.adventureDataKey);
    }
}
