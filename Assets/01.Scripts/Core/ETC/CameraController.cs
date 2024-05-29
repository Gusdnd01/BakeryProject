using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;
using DG.Tweening;

public class CameraController : MonoBehaviour
{
    private CinemachineVirtualCamera _vCam;
    private PoolVCam _poolVCam;
    private Transform _target;

    public PoolVCam CaomObj { get; private set; }
    public BattleController BattleController { get; set; }
    private Dictionary<CameraTargetType, Action<float, float, Ease>> _targetActionDic = new ();
    private bool _camOnMoving = false;

    private Sequence _toPlayerSeq;
    private Sequence _toEnemySeq;

    private void Awake()
    {
        _target = GameManager.Instance.gameObject.transform.Find("CameraTrm");
        _vCam = UIManager.Instance.VirtualCamera;
    }

    private void Start()
    {
        _targetActionDic.Add(CameraTargetType.Player, HandleCamraTargettingPlayer);
        _targetActionDic.Add(CameraTargetType.Enemy, HandleCamraTargettingEmeny);
    }

    private void SequenceClear()
    {
        _toPlayerSeq.Kill();
        _toEnemySeq.Kill();

        _toPlayerSeq = DOTween.Sequence();
        _toEnemySeq = DOTween.Sequence();
    }

    private void HandleCamraTargettingPlayer(float value, float duration, Ease easing)
    {
        Debug.Log("HELLO");
        _toPlayerSeq.Append(_target.DOLocalMoveX(value, duration).SetEase(easing));
        _toPlayerSeq.Join(_poolVCam.transform.DORotate(new Vector3(0, 0, 0.5f), duration).SetEase(easing));
        _toPlayerSeq.Join(DOTween.To(() => 5, o => _vCam.m_Lens.OrthographicSize = o, 5, duration).SetEase(easing));
        _toPlayerSeq.OnComplete(() => _camOnMoving = true);
    }

    private void HandleCamraTargettingEmeny(float value, float duration, Ease easing)
    {
        _toEnemySeq.Append(_target.DOLocalMoveX(value, duration).SetEase(easing));
        _toEnemySeq.Join(_poolVCam.transform.DORotate(new Vector3(0, 0, -0.5f), duration).SetEase(easing));
        _toEnemySeq.Join(DOTween.To(() => 5, o => _vCam.m_Lens.OrthographicSize = o, 5, duration).SetEase(easing));
        _toEnemySeq.OnComplete(() => _camOnMoving = true);
    }

    public void SetTransitionTime(float time)
    {
        UIManager.Instance.CinemachineBrain.m_DefaultBlend.m_Time = time;
    }

    public void StartCameraSequnce(CameraMoveTypeSO moveType)
    {
        _poolVCam = PoolManager.Instance.Pop(PoolingType.VCamPool) as PoolVCam;
        _vCam = _poolVCam.VCam;
        _vCam.Follow = _target;

        StartCoroutine(CameraSequenceCo(moveType.camMoveSequenceList));
    }

    private IEnumerator CameraSequenceCo(List<CameraMoveSequence> sequenceList)
    {
        foreach(CameraMoveSequence seq in sequenceList)
        {
            SetTransitionTime(seq.cameraTransitionTime);
            _camOnMoving = false;
            SequenceClear();

            _targetActionDic[seq.cameraTarget].Invoke(seq.movingValue * (int)seq.cameraTarget, seq.duration, seq.easingType);

            yield return new WaitUntil(() => _camOnMoving);
        }

        PoolManager.Instance.Push(_poolVCam);
    }
}
