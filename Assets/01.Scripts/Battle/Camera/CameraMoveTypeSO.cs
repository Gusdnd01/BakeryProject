using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public enum CameraTargetType
{
    Enemy = -1,
    Player = 1,
}

[Serializable]
public struct CameraMoveSequence
{
    public CameraTargetType cameraTarget;
    public Ease easingType;
    public float movingValue;
    public float duration;
    public float cameraTransitionTime;
}

[CreateAssetMenu(menuName = "SO/Camera/Sequence")]
public class CameraMoveTypeSO : ScriptableObject
{
    public List<CameraMoveSequence> camMoveSequenceList = new(); 
}
