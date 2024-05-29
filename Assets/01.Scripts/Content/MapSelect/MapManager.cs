using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChapterDefine;
using DG.Tweening;
using System.Linq;
using System;

public class MapManager : MonoBehaviour
{
    private static MapManager _instance;
    public static MapManager Instanace
    {
        get
        {
            if (_instance != null) return _instance;
            _instance = FindObjectOfType<MapManager>();
            if (_instance == null)
            {
                Debug.LogError("Not Exist MapManager");
            }
            return _instance;
        }
    }

    public MapDataSO SelectMapData { get; set; }
    public StageDataSO SelectStageData { get; set; }
    public List<CardBase> SelectDeck { get; set; }
    [SerializeField] private StageBubble _stageBubblePrefab;
    private StageBubble _stageBubbleObject;

    public bool isOnPanel;
    public bool isOnLoadMap;

    public int SelectStageNumber { get; set; }
    public GameObject LoadMapObject { get; set; }

    public void CreateStageInfoBubble(string stageName, Vector2 pos, bool isReverse)
    {
        if (_stageBubbleObject != null)
        {
            Destroy(_stageBubbleObject.gameObject);
        }

        Transform parent = LoadMapObject.transform.Find("BubbleParent");
        _stageBubbleObject = Instantiate(_stageBubblePrefab, parent);
        _stageBubbleObject.transform.localPosition = pos + new Vector2(0, 20);
        _stageBubbleObject.transform.DOLocalMove(pos, 0.3f);

        _stageBubbleObject.SetInfo(stageName, isReverse);
    }

}
