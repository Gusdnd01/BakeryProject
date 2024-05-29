using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class DialogueEffect : PoolableMono
{
    [SerializeField] private Image _bubble;
    [SerializeField] private Image _effectElement;
    [SerializeField] private float _fadingTime;

    [SerializeField] private AnimatorOverrideController _effectAnimator;
    [SerializeField] private Animator _animator;

    private readonly int _starthash = Animator.StringToHash("isStart");

    public void StartEffect(Sprite img, AnimationClip clip, CharacterStandard currentCharacter)
    {
        _effectAnimator["NormalClip"] = clip;
        _effectElement.sprite = img;

        _animator.SetBool(_starthash, true);
        EpiswordMaster.SetEmotionReactionPos(transform, currentCharacter);
        
        Sequence seq = DOTween.Sequence();
        seq.Append(_bubble.DOFade(1, _fadingTime));
        seq.Join(_effectElement.DOFade(1, _fadingTime));
    }

    public void StartEffect(Sprite img, AnimationClip clip)
    {
        _effectAnimator["NormalClip"] = clip;
        _effectElement.sprite = img;

        _animator.SetBool(_starthash, true);

        Sequence seq = DOTween.Sequence();
        seq.Append(_bubble.DOFade(1, _fadingTime));
        seq.Join(_effectElement.DOFade(1, _fadingTime));
    }

    public void EndEffect()
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(_bubble.DOFade(0, _fadingTime));
        seq.Join(_effectElement.DOFade(0, _fadingTime));
        seq.AppendCallback(() => PoolManager.Instance.Push(this));
    }

    public override void Init()
    {
        _bubble.color = new Color(1, 1, 1, 0);
        _effectElement.color = new Color(1, 1, 1, 0);
        _effectElement.sprite = null;
        _animator.SetBool(_starthash, false);
    }
}
