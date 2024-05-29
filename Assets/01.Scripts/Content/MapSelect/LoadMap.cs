using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadMap : MonoBehaviour
{
    [SerializeField] private Transform _nodeMapParent;
    [SerializeField] private MapNode[] _mapNodeArr;

    [SerializeField] private Transform _loadMapTrm;

    private void Start()
    {
        _loadMapTrm.transform.localScale = new Vector3(0, 1, 1);

        AdventureData adData = UIManager.Instance.GetSceneUI<SelectMapUI>().GetAdventureData();

        int myChapterIdx = (int)MapManager.Instanace.SelectMapData.myChapterType;

        int chapterIdx = Convert.ToInt16(adData.InChallingingStageCount.Split('-')[0]) - 1;
        
        if(myChapterIdx < chapterIdx)
        {
            foreach(MapNode node in _mapNodeArr)
            {
                node.gameObject.SetActive(true);
            }
        }
        else
        {
            int stageIdx = Convert.ToInt16(adData.InChallingingStageCount.Split('-')[1]);

            for(int i = 0; i < stageIdx; i++)
            {
                _mapNodeArr[i].gameObject.SetActive(true);
            }
        }

        _loadMapTrm.DOKill();
        _loadMapTrm.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
    }
}
