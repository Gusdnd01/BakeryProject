using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(menuName = "SO/FormationInfo")]
public class FormationSO : ScriptableObject
{
    public bool[,] isStuck;
    public int enemyCnt;
#if UNITY_EDITOR
    [CustomEditor(typeof(FormationSO), true)]
    public class FormationSOEditor : Editor
    {
        public bool[,] isStuck;
        private bool openIsStuck;
        private FormationSO script;
        private void OnValidate()
        {
            script.isStuck = new bool[script.enemyCnt, script.enemyCnt];
        }
        private void OnEnable()
        {
            script = target as FormationSO;
            script.isStuck = new bool[script.enemyCnt, script.enemyCnt];
    }
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            openIsStuck = EditorGUILayout.Foldout(openIsStuck, "DetectedSetting", true);
            if (openIsStuck)
            {
                for (int y = 0; y < script.enemyCnt; y++)
                {
                    EditorGUILayout.BeginHorizontal();
                    for (int x = 0; x < script.enemyCnt - x; x++)
                    {
                        script.isStuck[y, x] = EditorGUILayout.Toggle(script.isStuck[y,x]);
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
        }
    }
#endif

    private void OnEnable()
    {
    }
}
