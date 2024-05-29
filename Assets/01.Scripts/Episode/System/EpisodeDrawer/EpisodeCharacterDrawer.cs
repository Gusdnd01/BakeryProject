using System.Collections.Generic;
using UnityEngine;
using EpisodeDialogueDefine;
using System;
using UnityEngine.Events;
using DG.Tweening;

[Serializable]
public struct CharacterElementGroup
{
    public CharacterStandard characterStand;
    public RectTransform[] emotionTrm;
}

[Serializable]
public struct EmotionElementGroup
{
    public Sprite elementImg;
    public AnimationClip elementClip;
}

public class EpisodeCharacterDrawer : MonoBehaviour
{
    [SerializeField] private Transform _emotionTrm;
    [SerializeField] private CharacterElementGroup[] _characterGroupArr;
    [SerializeField] private EmotionElementGroup[] _emotionGroupArr;
    private Dictionary<CharacterType, CharacterStandard> _characterSelectDictionary = new Dictionary<CharacterType, CharacterStandard>();
    private CharacterStandard _selectCharacter;
    private SoundSelecter _episodeSounder;

    private void Awake()
    {
        _episodeSounder = transform.parent.Find("EpisodeSounder").GetComponent<SoundSelecter>();
        foreach(CharacterType ct in Enum.GetValues(typeof(CharacterType)))
        {
            _characterSelectDictionary.Add(ct, _characterGroupArr[(int)ct].characterStand);
        }
    }

    public void HandleCharacterDraw(CharacterType ct, FaceType faceType , CharacterActiveType activeType, bool isShake)
    {
        _selectCharacter = _characterSelectDictionary[ct];
        _selectCharacter.SetFace(faceType);

        switch (activeType)
        {
            case CharacterActiveType.Contain:
                _selectCharacter.SetActive(_selectCharacter.gameObject.activeSelf);
                break;
            case CharacterActiveType.None:
                _selectCharacter.SetActive(false);
                break;
            case CharacterActiveType.Active:
                _selectCharacter.SetActive(true);
                break;
        }

        if (isShake)
        {
            _selectCharacter.CharacterShake();
        }
    }

    public void HandleCharacterMoveDraw(CharacterType ct, Vector2 movePos, Quaternion rot)
    {
        if (movePos == Vector2.zero && rot == Quaternion.identity) return;

        _selectCharacter = _characterSelectDictionary[ct];
        _selectCharacter.MoveCharacter(movePos);
        _selectCharacter.transform.DOLocalRotateQuaternion(rot, 0.2f);
    }

    public void HandleDialogueEffectDraw(CharacterType ct, EmotionType emo)
    {
        if (emo == EmotionType.None) return;

        DialogueEffect de = PoolManager.Instance.Pop(PoolingType.DialogueEffect) as DialogueEffect;

        EmotionElementGroup eg = _emotionGroupArr[(int)emo - 1];
        de.StartEffect(eg.elementImg, eg.elementClip, _characterSelectDictionary[ct]);

        SFXType st = emo == EmotionType.Sparkle ? SFXType.sparcle : SFXType.bubble;
        _episodeSounder.HandleOutputSFX(st);
    }
}
