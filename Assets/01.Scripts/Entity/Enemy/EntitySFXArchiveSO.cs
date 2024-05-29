using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public struct CombatSoundGroup
{
    public AudioClip attackSound;
    public AudioClip hitSound;
    public AudioClip dieSound;
}

[Serializable]
public struct ElementSoundGroup
{
    public AudioClip moveSound;
    public AudioClip windSound;
}

[CreateAssetMenu(menuName = "SO/Sound/EntitySfxArchive")]
public class EntitySFXArchiveSO : ScriptableObject
{
    public CombatSoundGroup CombatSoundGroup;
    public ElementSoundGroup ElementSoundGroup;
}
