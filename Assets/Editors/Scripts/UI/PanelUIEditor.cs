using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(PanelUI), true)]
public class PanelUIEditor : Editor
{
    private PanelUI _targetPanelUI;

    private void OnEnable()
    {
        _targetPanelUI = (PanelUI)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        if (_targetPanelUI.useBlackPanel)
        {
            _targetPanelUI.blackPanel = EditorGUILayout.ObjectField("BlackPanel",
                                      _targetPanelUI.blackPanel,
                                      typeof(Image), true) as Image;

            _targetPanelUI.easingTime = EditorGUILayout.FloatField("EasingTime",
                                      _targetPanelUI.easingTime);

            _targetPanelUI.endOfAlpha = EditorGUILayout.FloatField("End of alpha",
                                      _targetPanelUI.endOfAlpha);

        }
        
        if (_targetPanelUI.canSeconderyActivePanel)
        {
            _targetPanelUI.toCreatedCount = EditorGUILayout.IntField("Beforehand Create Panel Count",
                                            _targetPanelUI.toCreatedCount);
        }

        if(GUI.changed)
        {
            EditorUtility.SetDirty(_targetPanelUI);
        }
    }
}
#endif