using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;

public class ChapterElement : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private MapDataSO _chapterData;
    [SerializeField] private UnityEvent<MapDataSO> _loadMapActiveEvent;

    [Header("ÂüÁ¶°ª")]
    [SerializeField] private TextMeshProUGUI _chapterNameTxt;
    [SerializeField] private TextMeshProUGUI _chapterInfoTxt;
    [SerializeField] private GameObject _lockPanel;

    private void Start()
    {
        _chapterNameTxt.text = _chapterData.chapterName;
        _chapterInfoTxt.text = _chapterData.chapterInfo;
    }

    public void SelectThisChapter()
    {
        MapManager.Instanace.SelectMapData = _chapterData;
        _loadMapActiveEvent?.Invoke(_chapterData);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        SelectThisChapter();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.DOKill();
        transform.DOScale(Vector3.one * 1.05f, 0.2f).SetEase(Ease.OutBack);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.DOKill();
        transform.DOScale(Vector3.one, 0.2f);
    }
}
