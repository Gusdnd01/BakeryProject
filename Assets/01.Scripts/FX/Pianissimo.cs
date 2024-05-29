using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Pianissimo : MonoBehaviour
{
    public Transform target;
    private float _speed = 0.0f;

    private void Start()
    {
        Ready();
    }

    private void Update()
    {
        transform.position += Vector3.up * _speed;
    }

    private void Ready()
    {
        Vector3 dir = (transform.position - target.position).normalized;
        Quaternion quat = Quaternion.LookRotation(dir);

        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DORotateQuaternion(quat, 1.0f));
        seq.AppendCallback(() =>
        {
            Attack();
        });
    }

    private void Attack()
    {
        _speed = 1.0f;
    }
}
