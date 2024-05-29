using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAppear : MonoBehaviour
{
    [SerializeField] private ParticleSystem _crackFX;

    private Player _player;
    private Vector2 _targetPos;

    private void Awake()
    {
        _targetPos = transform.position;
        transform.position += new Vector3(0, 20, 0);
        transform.localScale = new Vector3(0.5f, 1, 1);

        _player = GetComponent<Player>();
    }

    public void Action()
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOMove(_targetPos, 0.5f).SetEase(Ease.InQuart));
        seq.Join(transform.DOScale(Vector3.one, 0.5f));
        seq.AppendCallback(()=> _crackFX.gameObject.SetActive(true));
        seq.AppendCallback(()=> _crackFX.Play());
        seq.AppendCallback(()=> _player.cream.transform.SetParent(transform.parent));
    }
}
