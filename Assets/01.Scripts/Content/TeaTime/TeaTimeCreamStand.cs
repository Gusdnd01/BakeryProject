using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct HeartEffectEntity
{
    public Transform heartTrm;
    public Sprite heartSprite;
    public AnimationClip clip;
}

public class TeaTimeCreamStand : MonoBehaviour
{
    [SerializeField] private Image _standIllust;
    [SerializeField] private HeartEffectEntity _heartEntity;
    [SerializeField] private Sprite _eatFace;
    [SerializeField] private Sprite _normalFace;

    public void ChangeFace(bool isEatReady)
    {
        if (isEatReady)
        {
            _standIllust.sprite = _eatFace;
        }
        else
        {
            _standIllust.sprite = _normalFace;
        }
    }

    public void EatCake(ItemDataBreadSO cakeInfo)
    {
        Debug.Log(cakeInfo);
    }

    public void Reaction()
    {
        MakeHeartOicyEffect();

        Sequence seq = DOTween.Sequence();
        seq.Append(_standIllust.transform.DOLocalJump(new Vector3(40, -163), 50, 1, 0.5f));
        seq.AppendCallback(() =>
        {
            _standIllust.sprite = _normalFace;
            UIManager.Instance.GetSceneUI<TeaTimeUI>().DirectorStart();
        });
    }

    private void MakeHeartOicyEffect()
    {
        DialogueEffect de = PoolManager.Instance.Pop(PoolingType.DialogueEffect) as DialogueEffect;
        de.transform.SetParent(_heartEntity.heartTrm);
        de.transform.localScale = Vector3.one;
        de.transform.localPosition = Vector3.zero;
        de.StartEffect(_heartEntity.heartSprite, _heartEntity.clip);
    }
}
