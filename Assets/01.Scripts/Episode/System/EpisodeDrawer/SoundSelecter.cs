using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EpisodeDialogueDefine;
using UnityEngine.Events;

public class SoundSelecter : MonoBehaviour
{
    [SerializeField] private List<AudioClip> _sfxClipList = new List<AudioClip>();
    [SerializeField] private List<AudioClip> _bgmClipList = new List<AudioClip>();

    [SerializeField] private UnityEvent<BGMType, AudioClip> _bgmEvent = null;
    [SerializeField] private UnityEvent<AudioClip> _sfxEvent = null;

    public void HandleChangeBGM(BGMType bg)
    {
        //_bgmEvent?.Invoke(bg, _bgmClipList[(int)bg - 2]);
    }

    public void HandleOutputSFX(SFXType st)
    {
        //_sfxEvent?.Invoke(_sfxClipList[(int)st]);
    }
}
