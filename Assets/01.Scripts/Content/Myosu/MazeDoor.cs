using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MazeDoor : MonoBehaviour, IPointerEnterHandler, 
                                       IPointerExitHandler,
                                       IPointerClickHandler
{
    public StageDataSO AssignedStageInfo { get; set; }
    private CanvasGroup _visual;
    public CanvasGroup Visual
    {
        get
        {
            if(_visual == null)
            {
                _visual = GetComponent<CanvasGroup>();
            }
            return _visual;
        }
    }
    [SerializeField] private Transform _doorTrm;
    [SerializeField] private CompensationBubble _comBubble;

    [SerializeField] private UnityEvent<MazeDoor> _doorHoverEvent;
    [SerializeField] private UnityEvent<MazeDoor> _doorHoverOutEvent;
    [SerializeField] private UnityEvent<MazeDoor> _doorSelectEvent;

    private Vector3 _normalScale;
    private Tween _hoverTween;
    private Tween _shakeTween;

    public bool CanInteractible { get; set; } = true;

    private void Start()
    {
        _normalScale = transform.localScale;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!CanInteractible) return;

        _hoverTween.Kill();

        _hoverTween = transform.DOScale(transform.localScale * 1.1f, 0.3f);
        _shakeTween = transform.DOShakeRotation(1f, 3, 10).SetLoops(-1);
        _comBubble.SpeachUpBubble(AssignedStageInfo.compensation.Item.itemIcon, 50);

        _doorHoverEvent?.Invoke(this);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (!CanInteractible) return;

        _hoverTween.Kill();
        _shakeTween.Kill();

        transform.rotation = Quaternion.identity;
        _hoverTween = transform.DOScale(_normalScale, 0.3f);
        _comBubble.SpeachDownBubble();

        _doorHoverOutEvent?.Invoke(this);
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        CanInteractible = false;

        _comBubble.SpeachDownBubble();
        _hoverTween?.Kill();
        _shakeTween?.Kill();
        UIManager.Instance.GetSceneUI<MyosuUI>().HideText();

        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOScale(_normalScale * 1.3f, 1));
        seq.Join(transform.DOLocalMoveX(0, 1));
        seq.Join(_doorTrm.DOLocalRotateQuaternion(Quaternion.Euler(0, -90, 0), 1));
        seq.AppendCallback(() => 
        {
            MapManager.Instanace.SelectStageData = AssignedStageInfo;
            GameManager.Instance.ChangeScene(SceneType.battle);
        });
        _doorSelectEvent?.Invoke(this);
    }
}
