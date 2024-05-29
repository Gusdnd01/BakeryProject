using DG.Tweening;
using ExtensionFunction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Stove : MonoBehaviour, IBakingProductionObject
{
    public UnityEvent OnEndShaking;

    [SerializeField] private Vector2 _normalScale;
    public float EasingTime { get; set; } = 0.3f;

    public void OnProduction()
    {
        transform.SmartScale(_normalScale * 1.3f, EasingTime);
    }

    public void ExitProduction()
    {
        transform.SmartScale(_normalScale, EasingTime);
    }

    public void DoughInStove(int grade)
    {
        Debug.Log(grade);
        float shakeTime = 1, shakePower = 1, shakeSize = 1, scaleTime = 1;


        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(1f);
        for (int i = 0; i < 2; i++)
        {
            int dir = i % 2 == 0 ? 1 : -1;
            seq.Append(transform.DORotate(new Vector3(0, 0, dir * 20), EasingTime).SetEase(Ease.OutBack));
            seq.Join(transform.DOMoveX(-dir, EasingTime).SetEase(Ease.OutBack));
            seq.AppendInterval(0.5f);
        }
        seq.Append(transform.DORotate(Vector3.zero, EasingTime));
        seq.Join(transform.DOMoveX(0, EasingTime));

        switch (grade)
        {
            case 0:
                {
                    shakeTime = 3.5f;
                    shakePower = 12f;
                    shakeSize = 0.7f;
                    scaleTime = 2f;
                }
                break;
            case 1:
                {
                    shakeTime = 1.6f;
                    shakePower = 10f;
                    shakeSize = 0.75f;
                    scaleTime = 1.5f;
                }
                break;
            case 2:
                {
                    shakeTime = 1f;
                    shakePower = 7f;
                    shakeSize = 0.8f;
                    scaleTime = 1f;
                }
                break;
            default:
                break;
        }

        seq.Append(transform.DOShakeRotation(shakeTime, new Vector3(0, 0, shakePower), 10, 10, false, ShakeRandomnessMode.Harmonic));
        seq.Join(transform.DOScale(_normalScale * shakeSize, scaleTime).SetEase(Ease.OutQuad));
        seq.Join(transform.DOLocalMoveY(0, scaleTime).SetEase(Ease.OutQuad));

        seq.OnComplete(() =>
        {
            transform.DOScale(_normalScale, 0.5f).SetEase(Ease.InOutBack);
            OnEndShaking.Invoke();
        });
    }

    private void NormalGrade()
    {
/*        seq.Append(transform.DOShakeRotation(1f, new Vector3(0, 0, 7), 10, 10, false, ShakeRandomnessMode.Harmonic));
        seq.Join(transform.DOScale(_normalScale * 0.78f, 1f).SetEase(Ease.OutQuad));
        seq.Join(transform.DOLocalMoveY(0, 1f).SetEase(Ease.OutQuad));

        seq.OnComplete(() =>
        {
            OnEndShaking.Invoke();
            transform.DOScale(_normalScale * 0.83f, 0.45f).SetEase(Ease.InOutBack);
        });*/
    }

    private void EpicGrade()
    {
/*        seq.Append(transform.DOShakeRotation(2f, new Vector3(0, 0, 7), 12, 10, false, ShakeRandomnessMode.Harmonic));
        seq.Join(transform.DOScale(_normalScale * 0.75f, 2f).SetEase(Ease.OutQuad));
        seq.Join(transform.DOLocalMoveY(0, 2f).SetEase(Ease.OutQuad));

        seq.OnComplete(() =>
        {
            OnEndShaking.Invoke();
            transform.DOScale(_normalScale * 0.85f, 0.45f).SetEase(Ease.InOutBack);
        });*/
    }

    private void LegendGrade()
    {

    }
}
