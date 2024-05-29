using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

[CreateAssetMenu(menuName ="SO/Stage")]

#endif
public class StageInfoSO : LoadableData
{
#if UNITY_EDITOR
    [CustomEditor(typeof(StageInfoSO))]
    public class StageInfoLoader : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            StageInfoSO episodeData = (StageInfoSO)target;
            LoadableData ld = episodeData;
            if (GUILayout.Button("EpisodeDataReading"))
            {
                Debug.Log("DataReading Start . . .");
                //ld.GeneratDialogueData();
            }
            if (GUILayout.Button("DataGenerate"))
            {
                Debug.Log("DataGenerate Start . . .");
                ld.Generate();
            }
        }
    }
#endif

    public List<Data> datas = new List<Data>();

    public void GetList()
    {
        datas = generateData;
    }
}
