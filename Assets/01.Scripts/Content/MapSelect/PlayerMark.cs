using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerMark : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;

    private void Start()
    {
        transform.DOLocalMoveY(1.6f, 0.5f).SetLoops(-1, LoopType.Yoyo);
    }

    public void ActivePlayerMark(bool isActive)
    {
        _spriteRenderer.enabled = isActive;
    }
}
