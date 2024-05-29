using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossRoadsAppearText : MonoBehaviour
{
    [SerializeField] private float _eulerZAngle;

    public void Show()
    {
        transform.localScale = Vector3.zero;
        transform.localRotation = Quaternion.Euler(new Vector3(0, 0, _eulerZAngle));

        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOScale(1.4f, 0.2f));
        seq.Join(transform.DOLocalRotateQuaternion(Quaternion.Euler(new Vector3(0, 0, -(_eulerZAngle) * 0.5f)), 0.2f));
        seq.AppendInterval(0.05f);
        seq.Append(transform.DOScale(1, 0.2f));
        seq.Join(transform.DOLocalRotateQuaternion(Quaternion.identity, 0.2f));
    }

    public void Hide()
    {
        transform.DOScale(0f, 0.2f);
    }
}
