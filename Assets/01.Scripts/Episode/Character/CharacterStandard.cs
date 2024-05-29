using UnityEngine;
using EpisodeDialogueDefine;
using DG.Tweening;
using System;
using UnityEngine.UI;

public class CharacterStandard : MonoBehaviour
{
    [Header("수치")]
    [SerializeField] private float _activeTime;
    [SerializeField] private float _moveTime;
    [SerializeField] private float _exitTime;

    [Header("셋팅값")]
    [SerializeField] private Image _characterDraw;
    [SerializeField] private Sprite[] _faceGroup;

    [field:SerializeField] public Transform EmotionRightPos { get; private set; }
    [field:SerializeField] public Transform EmotionLeftPos { get; private set; }

    private Tween _moveTween;
    private FaceType _currentFaceType;
    private bool _currentActive;
    private Vector2 _alreadyInPos;

    public void SetFace(FaceType faceType)
    {
        if (_currentFaceType == faceType) return;

        _characterDraw.sprite = _faceGroup[(int)faceType];
        _currentFaceType = faceType;
    }

    public void SetActive(bool isActive)
    {
        if (_currentActive == isActive) return;
        _characterDraw.DOFade(Convert.ToInt32(isActive), _activeTime);
        _currentActive = isActive;
    }

    public void CharacterShake()
    {
        transform.DOShakePosition(0.4f, 15f, 20);
    }

    public void MoveCharacter(Vector2 pos)
    {
        Debug.Log(pos);
        if (_alreadyInPos == pos) return;

        _moveTween.Kill();
        _moveTween = transform.DOLocalMove(pos, _moveTime);
        _alreadyInPos = pos;
    }
}
