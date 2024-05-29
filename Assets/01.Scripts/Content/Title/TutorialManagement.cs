using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManagement : MonoBehaviour
{
    private const string _dataKey = "checkFirstDatakey";

    private void Start()
    {
        if(DataManager.Instance.IsHaveData(_dataKey))
        {
            DataManager.Instance.CheckOnFirstData 
          = DataManager.Instance.LoadData<CheckOnFirst>(_dataKey);
        }
        else
        {
            DataManager.Instance.CheckOnFirstData = new ();
        }
    }
}
