using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

[CreateAssetMenu(menuName = "SO/Ba")]
#endif

public class Bakery : LoadableData
{
#if UNITY_EDITOR
    [CustomEditor(typeof(Bakery))]
    public class EpisodeLoader : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            Bakery episodeData = (Bakery)target;
            LoadableData ld = episodeData;
            
            if (GUILayout.Button("DataGenerate"))
            {
                Debug.Log("DataGenerate Start . . .");
                ld.Generate();
            }
        }
    }
#endif
}
