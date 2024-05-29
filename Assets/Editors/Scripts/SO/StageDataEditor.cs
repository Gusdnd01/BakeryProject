using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(StageDataSO), true)]
public class StageDataEditor : Editor
{
    private StageDataSO _stageDataSO;

    private void OnEnable()
    {
        _stageDataSO = (StageDataSO)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        //if (_stageDataSO.stageType == StageType.Mission)
        //{
        //    _stageDataSO.missionDeck = EditorGUILayout.ObjectField("MissionDeck",
        //                                               _stageDataSO.missionDeck,
        //                                               typeof(Object), true) as List<CardBase>;
        //}
    }
}
#endif