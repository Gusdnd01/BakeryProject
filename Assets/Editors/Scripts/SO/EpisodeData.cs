using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using EpisodeDialogueDefine;
using System;

[Serializable]
public struct DialogueStandardElement
{
    public string name;
    public string sentence;
    public BackGroundType backGroundType;

    public DialogueStandardElement(string n, string s, BackGroundType bt)
    {
        name = n;
        sentence = s;
        backGroundType = bt;
    }
}
[Serializable]
public struct DialogueCharacterElement
{
    public FaceType faceType;
    public bool isShake;
    public EmotionType emotionType;

    public DialogueCharacterElement(FaceType ft, bool sh, EmotionType et)
    {
        faceType = ft;
        isShake = sh;
        emotionType = et;
    }
}
[Serializable]
public struct DialogueProductElement
{
    public FadeOutType fadeType;

    public DialogueProductElement(FadeOutType ft)
    {
        fadeType = ft;
    }
}
[Serializable]
public struct CaptureElement
{
    public CharacterActiveType activeType;
    public Vector2 movePosition;
    public Quaternion rotationValue;

    public CaptureElement(CharacterActiveType _activeType, Vector2 _movePosition, Quaternion _rotationValue)
    {
        activeType = _activeType;
        movePosition = _movePosition;
        rotationValue = _rotationValue;
    }
}

[Serializable]
public struct DialogueElement
{
    public DialogueStandardElement standardElement;
    public DialogueCharacterElement characterElement;
    public DialogueProductElement productElement;
    public CaptureElement captureElement;
    public Sprite priviewSprite;
    public bool isLinker;

    public DialogueElement(DialogueStandardElement  _sElement,
                    DialogueCharacterElement _cElement,
                    DialogueProductElement   _pElement,
                    CaptureElement _capElement,
                    Sprite _priviewSprite,
                    bool _linker)
    {
        standardElement = _sElement;
        characterElement = _cElement;
        productElement = _pElement;
        captureElement = _capElement;
        priviewSprite = _priviewSprite;
        isLinker = _linker;
    }
}

#if UNITY_EDITOR
[CreateAssetMenu(menuName = "SO/Episode/Dialogue")]
#endif
public class EpisodeData : LoadableData
{
    public EpisodeCaptureData captureData;
    public List<DialogueElement> dialogueElement = new List<DialogueElement>();

    public void GeneratDialogueData()
    {
        dialogueElement.Clear();
        for(int i = 0; i < generateData.Count; i++)
        {
            dialogueElement.Add
            (
                new DialogueElement
                (
                    AllocateSE(generateData[i].str[0], generateData[i].str[1], generateData[i].str[2]),
                    AllocateCE(generateData[i].str[4], generateData[i].str[5], generateData[i].str[6]),
                    AllocatePE(generateData[i].str[3]),
                    new CaptureElement(CharacterActiveType.Contain, Vector2.zero, Quaternion.identity),
                    null,
                    generateData[i].str[1].Contains("link")
                )
            ); 
        }
        Debug.Log("Complete DataReading!!");
    }
    public void GenerateCaptureData()
    {
        if(captureData == null)
        {
            Debug.LogError("캡쳐 데이터가 없는데숭?");
            return;
        }

        foreach(CaptureDataBox box in captureData.episodeCaptureElement)
        {
            if(box.captureDataIDX >= dialogueElement.Count)
            {
                Debug.LogError("다이얼로그 리스트 확인 요함");
                return;
            }

            DialogueElement de = dialogueElement[box.captureDataIDX];
            de.captureElement = box.captureElement;
            dialogueElement[box.captureDataIDX] = de;
        }
    }
    private DialogueStandardElement AllocateSE(string n, string s, string bt)
    {
        return new DialogueStandardElement
            (
                n, s,
                (BackGroundType)Enum.Parse(typeof(BackGroundType), bt)
            );
    }
    private DialogueCharacterElement AllocateCE(string ft, string sh, string et)
    {
        return new DialogueCharacterElement
            (
                (FaceType)Enum.Parse(typeof(FaceType), ft),
                Convert.ToBoolean(sh),
                (EmotionType)Enum.Parse(typeof(EmotionType), et)
            ) ; 
    }
    private DialogueProductElement AllocatePE(string ft)
    {
        return new DialogueProductElement((FadeOutType)Enum.Parse(typeof(FadeOutType), ft));
    }

}

#if UNITY_EDITOR
[CustomEditor(typeof(EpisodeData))]
public class EpisodeLoader : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EpisodeData episodeData = (EpisodeData)target;
        LoadableData ld = episodeData;
        if (GUILayout.Button("EpisodeDataReading"))
        {
            Debug.Log("DataReading Start . . .");
            episodeData.GeneratDialogueData();
        }
        if (GUILayout.Button("CaptureDataReading"))
        {
            Debug.Log("CaptureReading Start . . .");
            episodeData.GenerateCaptureData();
        }
        if (GUILayout.Button("DataGenerate"))
        {
            Debug.Log("DataGenerate Start . . .");
            ld.Generate();
        }
    }
}
#endif
