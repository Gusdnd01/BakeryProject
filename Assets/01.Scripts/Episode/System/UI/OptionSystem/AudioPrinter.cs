using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EpisodeDialogueDefine;

public class AudioPrinter : MonoBehaviour
{
    [SerializeField] private AudioSource _bgmAudioSource;
    [SerializeField] private AudioSource _sfxAudioSource;

    private float _mainValue;
    public float MainValue
    {
        get
        {
            return _mainValue;
        }
        set
        {
            _mainValue = value;
        }
    }

    public void HandlePrintBgm(BGMType bg, AudioClip clip)
    {
        if (bg == BGMType.Contain) return;

        _bgmAudioSource.Stop();
        if (bg == BGMType.None) return;

        _bgmAudioSource.clip = clip;
        _bgmAudioSource.Play();
    }

    public void HandlePrintSfx(AudioClip clip)
    {
        _sfxAudioSource.PlayOneShot(clip);
    }

    public void SetBgmVolume(float bValue)
    {
        _bgmAudioSource.volume = bValue * _mainValue;
    }

    public void SetSfxVolume(float sValue)
    {
        _sfxAudioSource.volume = sValue * _mainValue;
    }
}
