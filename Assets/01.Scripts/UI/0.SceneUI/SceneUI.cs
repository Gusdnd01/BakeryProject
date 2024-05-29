using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UIDefine;

public class SceneUI : MonoBehaviour
{
    [SerializeField] private SceneType _myType;
    public SceneType ScreenType => _myType;

    [SerializeField] private AudioClip _audioClip;

    public virtual void SceneUIStart()
    {
        StartCoroutine(DelayCo(.5f));
    }

    public virtual void SceneUIEnd()
    {
        SoundManager.Instance.StopBGM();
    }

    IEnumerator DelayCo(float delay)
    {
        yield return new WaitForSeconds(delay);
        SoundManager.PlayAudio(_audioClip, true);
    }
}
