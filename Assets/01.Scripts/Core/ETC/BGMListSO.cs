using UnityEngine;
using System;

[Serializable]
public struct Battle
{
    public AudioClip battle_1;
    public AudioClip battle_2;
    public AudioClip battle_3;
    public AudioClip battle_4;
    public AudioClip battle_5;
}

[CreateAssetMenu(menuName ="SO/Sound/BGMList")]
public class BGMListSO : ScriptableObject
{
    public Battle battleType;
}
