using EpisodeDialogueDefine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[Serializable]
public struct CaptureDataBox
{
    public int captureDataIDX;
    public CaptureElement captureElement;

    public CaptureDataBox(int idx, CaptureElement ce)
    {
        captureDataIDX = idx;
        captureElement = ce;
    }
}

#if UNITY_EDITOR
[CreateAssetMenu(menuName = "SO/Episode/PositionData")]
#endif
public class EpisodeCaptureData : ScriptableObject
{
    public int toCaptureIDX;
    public List<CaptureDataBox> episodeCaptureElement = new List<CaptureDataBox>();

    public void CaptureCharacterElement(CharacterType characterType)
    {
        string cPath = "ManagerGroup/EpisodeManager/EpisodeGroup/UICANVAS/CharacterGroup";

        if (characterType == CharacterType.tart)
        {
            cPath += "/Tart";
        }
        else
        {
            cPath += "/Mawang";
        }
        CharacterStandard selectCharacter = GameObject.Find(cPath).GetComponent<CharacterStandard>();

        CharacterActiveType activeType = (CharacterActiveType)(Convert.ToInt16(selectCharacter.gameObject.activeSelf) + 1);
        CaptureElement ce = new CaptureElement(activeType,
                                               selectCharacter.transform.localPosition,
                                               selectCharacter.transform.localRotation);

        CaptureDataBox box = new CaptureDataBox(toCaptureIDX - 1, ce);
        episodeCaptureElement.Add(box);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(EpisodeCaptureData))]
public class EpisodePositionLoader : Editor
{
    public CharacterType CaptureCharacterType;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EpisodeCaptureData captureData = (EpisodeCaptureData)target;

        GUILayout.Label("캡쳐할 캐릭터 선택");
        GUIContent[] enumOptions = new GUIContent[Enum.GetNames(typeof(CharacterType)).Length];
        for (int i = 0; i < enumOptions.Length; i++)
        {
            enumOptions[i] = new GUIContent(Enum.GetNames(typeof(CharacterType))[i]);
        }
        CaptureCharacterType = (CharacterType)GUILayout.SelectionGrid((int)CaptureCharacterType, enumOptions, 1);

        if (GUILayout.Button("CaptureCharacterPose"))
        {
            captureData.CaptureCharacterElement(CaptureCharacterType);
            Debug.Log("Cheez :)");
        }
    }
}
#endif
