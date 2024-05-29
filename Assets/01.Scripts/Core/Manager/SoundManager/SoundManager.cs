using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;
using System.Collections.Concurrent;

public class SoundManager : MonoSingleton<SoundManager>
{
    [HideInInspector]
    public AudioPoolObject currentBgmObject;

    public static void PlayAudio(AudioClip clip, bool isLoop = false, float pitch = 1f, float volume = 1f)
    {
        PoolManager.Instance.Pop(PoolingType.Sound).GetComponent<AudioPoolObject>().Play(clip, pitch, volume, isLoop);
    }
    public static void PlayAudioRandPitch(AudioClip clip, bool isLoop = false, float pitch = 1f, float randValue = 0.1f, float volume = 1f)
    {
        PoolManager.Instance.Pop(PoolingType.Sound).GetComponent<AudioPoolObject>().Play(clip, pitch + Random.Range(-randValue, randValue), volume, isLoop);
    }
    public void StopBGM()
    {
        if (currentBgmObject == null) return;
        currentBgmObject.StopMusic();
    }
}