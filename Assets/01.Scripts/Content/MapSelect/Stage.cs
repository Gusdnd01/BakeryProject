using System;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class Stage : MonoBehaviour
{
    public EnemyGroupSO enemyGroup;
    public Transform enemySpawnTrm;

    public int stageIndex = 0;
    public CinemachineVirtualCamera vCam;
    public StageInfoSO stageInfo;
    
    private List<Transform> _stageInfo;
    public int maximumPhase = 3;//기본값 3

    public PolygonCollider2D[] confiners;

    [HideInInspector]
    public int curConfinerIndex = 0;

    public Transform camTrmsParent;

    //[HideInInspector]
    private bool _curPhaseCleared = false;
    public bool CurPhaseCleared
    {
        get
        {
            return _curPhaseCleared;
        }
        set
        {
            Debug.Log("Change Value");
            _curPhaseCleared = value;
        }
    }

    public Action OnStageStarted = null;
    public Action OnPhaseCleared = null;
    public Action OnStageCleared = null;

    private static float halfHeight = 0;
    private static float halfWidth = 0;

    private BoxCollider2D[] stageCollider = {null, null};

    public int CurPhase { get; set; }

    protected virtual void Awake()
    {
        //_stageInfo = new List<Transform>();

        // halfHeight = Camera.main.orthographicSize;
        // halfWidth= Camera.main.aspect * halfHeight;

        //OnStageCleared += Print;

        //for(int i = 0; i < 2; i++)
        //{
        //    GameObject obj = new GameObject($"mapCollider_{i}");

        //    obj.transform.localScale = new Vector3(1, 20,1);
            
        //    stageCollider[i] = obj.AddComponent<BoxCollider2D>();

        //    obj.transform.position = new Vector2((halfWidth * 2) * i - halfWidth, 0);
        //    obj.transform.SetParent(Camera.main.transform, false);
        //}

        //stageCollider[1].gameObject.AddComponent<PhaseMove>();
    }

    //debug
    private void Print()
    {
        print("stage Cleared!");
    }

    protected virtual void Start()
    {
        //stageInfo.GetList();

        //StageInfoGenerate();
        //OnStageStarted?.Invoke();
    }

    private void Update()
    {
        //디버그 코드
        if (Keyboard.current.gKey.wasPressedThisFrame)
        {
            PhaseCleared();
        }

    }

    /// <summary>
    /// 카메라 크기 만큼 이동하는 거?
    /// </summary>
    private void StageInfoGenerate()
    {
        int generateConfinerIndex = 0;
        for(int i = 0; i < maximumPhase; ++i)
        {
            if(int.Parse(stageInfo.datas[stageIndex].str[i]) == 0)
            {
                Transform trm = new GameObject().transform;
                trm.name = $"camTrm_{i + 1}";
                if(i == 0)
                {
                    trm.position = new Vector3(i * (halfWidth * 2.0f), 0, 0);
                }
                else
                {
                    trm.position = _stageInfo[i - 1].position + new Vector3(halfWidth * 2.0f, 0, 0);
                }
                trm.SetParent(camTrmsParent);
                _stageInfo.Add(trm);
            }
            else
            {
                //_stageInfo.Add(GameManager.Instance.PlayerTrm);

                Transform trm = new GameObject().transform;
                trm.name = $"camTrm_{i + 1}";
                trm.position = new Vector3(i * halfWidth * 2.0f + confiners[generateConfinerIndex].bounds.size.x, 0, 0);
                trm.SetParent(camTrmsParent);
                _stageInfo.Add(trm);

                generateConfinerIndex++;

                i++;
            }
        }

        vCam.m_Follow = _stageInfo[CurPhase];
    }

    /// <summary>
    /// 한 페이즈가 끝났을 때에 실행하는 거
    /// </summary>
    public void PhaseCleared()
    {
        if(CurPhase >= maximumPhase)
        {
            OnStageCleared?.Invoke();
            return;
        }

        OnPhaseCleared?.Invoke();
        CurPhase++;

        if(CurPhase < maximumPhase)
        {
            vCam.m_Follow = _stageInfo[CurPhase];
        }
        CurPhaseCleared = false;
    }
}
