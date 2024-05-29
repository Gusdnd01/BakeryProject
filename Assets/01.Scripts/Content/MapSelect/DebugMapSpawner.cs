using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugMapSpawner : MonoBehaviour
{
    public Transform mapTrmsParent;
    public Transform btnParent;
    public Transform phaseBtnParent;

    public Stage stage;

    //include prefabs
    public GameObject[] objs;
    private List<GameObject> _curObjects = new List<GameObject>();

    private int _curStageIndex = 0;

    private Button[] _btns;
    private Button[] _phaseBtns;

    private void Awake()
    {
        _btns = btnParent.GetComponentsInChildren<Button>();
        
        for(int i = 0; i < _btns.Length; ++i)
        {
            int index = i;
            _btns[index].onClick.AddListener(() => StageIns(index));
        }

        _phaseBtns = phaseBtnParent.GetComponentsInChildren<Button>();

        for(int i = 0; i < _phaseBtns.Length; ++i)
        {
            int index = i;
            _phaseBtns[index].onClick.AddListener(()=> PhaseMove(index) );
        }
    }

    private void Start()
    {
        foreach(var o in objs)
        {
            GameObject obj = Instantiate(o);
            obj.transform.position = Vector3.zero;
            obj.transform.SetParent(mapTrmsParent);

            _curObjects.Add(obj);
            obj.SetActive(false);
        }
    }

    //Button Event
    public void StageIns(int index)
    {
        _curObjects[_curStageIndex].SetActive(false);
        _curObjects[index].SetActive(true);
    }

    //Phase Event
    public void PhaseMove(int index)
    {
        stage.CurPhase = index-1;
        stage.PhaseCleared();
    }
}
