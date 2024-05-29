using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PhaseMove : MonoBehaviour
{
    private Stage _curStage = null;
    private CinemachineConfiner2D _cinemachineConfiner = null;

    private void Start()
    {
        _curStage = FindObjectOfType<Stage>();
        _cinemachineConfiner = _curStage.vCam.GetComponent<CinemachineConfiner2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(_curStage.CurPhaseCleared && collision.collider.CompareTag("Player"))
        {
            if(_curStage.TryGetComponent<BattleTutorial>(out BattleTutorial b))
            {
                _curStage.PhaseCleared();
                b.QuaterEnd("Quater End");

                //GameManager.Instance.PlayerTrm.position +=Vector3.right * 2;
            }
            else
            {
                _curStage.PhaseCleared();
                if (int.Parse(_curStage.stageInfo.datas[_curStage.stageIndex].str[_curStage.CurPhase]) == 1)
                {
                    print("콘파이너 적용");
                    _cinemachineConfiner.m_BoundingShape2D = _curStage.confiners[_curStage.curConfinerIndex];
                    _curStage.curConfinerIndex++;
                }
                else
                {
                    _cinemachineConfiner.m_BoundingShape2D = null;
                }
                ////GameManager.Instance.PlayerTrm.position += Vector3.right * 2;
            }

            _curStage.CurPhaseCleared = false;
        }
        else
        {
            return;
        }
    }
}
