using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class SoundData : CanSaveData
{
    public float MasterVoume;
    public float BgmVolume;
    public float SfxVolume;

    public override void SetInitialValue()
    {
        MasterVoume = 100;
        BgmVolume = 50;
        SfxVolume = 50;
    }
}
