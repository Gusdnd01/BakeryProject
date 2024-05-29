using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class DataManager : MonoSingleton<DataManager>
{
    private List<string> _datakeyList = new List<string>();
    private readonly string _dataKeyFilePath = Path.Combine(Application.dataPath, "DataKeys.json");

    public CheckOnFirst CheckOnFirstData { get; set; }

    private void Awake()
    {
        if(File.Exists(_dataKeyFilePath))
        {
            string[] keyArr = File.ReadAllText(_dataKeyFilePath).Split(",");
            int saveFileCount = keyArr.Length - 1;

            for (int i = 0; i < saveFileCount; i++)
            {
                _datakeyList.Add(keyArr[i]);
            }
        }
    }

    public void SaveData(CanSaveData saveData, string dataKey) 
    {
        if(!IsHaveData(dataKey))
        {
            string prevData = "";
            if(File.Exists(_dataKeyFilePath))
            {
                prevData = File.ReadAllText(_dataKeyFilePath);
            }
            string saveKey = prevData + $"{dataKey},";

            File.WriteAllText(_dataKeyFilePath, saveKey);
            _datakeyList.Add(dataKey);
        }

        File.WriteAllText(GetFilePath(dataKey), JsonUtility.ToJson(saveData));
    }

    public T LoadData<T>(string dataKey) where T : CanSaveData
    {
        if (!IsHaveData(dataKey))
        {
            Debug.LogWarning($"Error! No exit data key!! Key name : {dataKey}");
            return default(T);
        }
        return JsonUtility.FromJson<T>(File.ReadAllText(GetFilePath(dataKey)));
    }

    public bool IsHaveData(string dataKey)
    {
        return _datakeyList.Contains(dataKey);
    }

    private string GetFilePath(string dataKey)
    {
        return Path.Combine(Application.dataPath, $"{dataKey}.json");
    }
}
