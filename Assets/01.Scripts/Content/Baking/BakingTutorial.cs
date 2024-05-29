using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BakingTutorial : MonoBehaviour
{
    // 처음 켜는 거면 설명해주는 패널이 떠야함

    [SerializeField]
    private List<GameObject> _tutorialPanelList = new List<GameObject>();
    private int _curIdx = 0;

    public void Start()
    {
        if(DataManager.Instance.CheckOnFirstData.isFirstOnBaking)
        {
            _tutorialPanelList[_curIdx].SetActive(true);
        }
        else
        {
            CloseThePanel();
        }
    }

    private void EntryIntoBaking()
    {
        // 대충 이제 케이크 처만들어 라고 말하는 내용 출력
    }

    private void TurnOnThePanel(int currentIdx, int nextIdx)
    {
        _tutorialPanelList[currentIdx].SetActive(false);
        _tutorialPanelList[nextIdx].SetActive(true);
    }

    public void TurnThePanelToBefore()
    {
        if(_curIdx > 0)
        {
            TurnOnThePanel(_curIdx, --_curIdx);
        }
    }

    public void TurnThePanalToNext()
    {
        if(_curIdx < _tutorialPanelList.Count - 1)
        {
            TurnOnThePanel(_curIdx, ++_curIdx);
        }
    }

    public void CloseThePanel()
    {
        //_tutorialPanelList[_curIdx].SetActive(false);
        gameObject.SetActive(false);

        DataManager.Instance.CheckOnFirstData.isFirstOnBaking = false;
        DataManager.Instance.SaveData(DataManager.Instance.CheckOnFirstData, 
                                      DataKeyList.checkIsFirstPlayGameDataKey);
    }
}
