using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CardDefine;

public enum TargetEnemyCount
{
    I = 1,
    II,
    III,
    IV,
    V,
    ALL,
    ME
}

[CreateAssetMenu(menuName = "SO/Card/CardInfo")]
public class CardInfo : ScriptableObject
{
    [Header("카드 정보")]
    public string CardName;
    public CardType CardType;
    public Sprite CardVisual;

    [Header("스킬 정보")]
    public Color skillPersonalColor = Color.white;
    public AnimationClip abilityAnimation;
    public CardShameElementSO cardShameData;
    public CameraMoveTypeSO cameraSequenceData;

    [Header("개별 타격 이펙트")]
    public ParticleSystem hitEffect;
    public ParticleSystem targetEffect;

    [TextArea]
    public string AbillityInfo;
}
