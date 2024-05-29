using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

public class AbilityTargetArrow : MonoBehaviour
{
    [SerializeField] private Image _chainArrowVisual;
    [SerializeField] private Image[] _chainVisual;
    [SerializeField] private RectTransform _arrowMask;

    private Transform _saveStartTrm;
    private Vector2 _saveEndPos;
    private float _saveLength;
    
    private Sequence _fadeSequence;
    public bool IsGenerating { get; set; }
    public bool IsBindSucess { get; private set; }

    public Entity MarkingEntity { get; private set; }

    private float _inChaingingValue;

    public void ActiveArrow(bool value)
    {
        _chainArrowVisual.enabled = value;
    }

    public void SetFade(float fadeValue)
    {
        foreach (var chain in _chainVisual)
        {
            chain.color = new Color(chain.color.r, chain.color.g, chain.color.b, _inChaingingValue);
        }
        _fadeSequence.Kill();

        _inChaingingValue = fadeValue;
        _fadeSequence = DOTween.Sequence();

        foreach(var chain in _chainVisual)
        {
            _fadeSequence.Join(chain.DOFade(fadeValue, 0.5f));
        }
    }

    public void SetActive()
    {
        _fadeSequence.Kill();

        _fadeSequence = DOTween.Sequence();

        foreach (var chain in _chainVisual)
        {
            _fadeSequence.Join(chain.DOFade(1f, 0.2f));
        }
    }

    private void Update()
    {
        if(IsGenerating)
        {
            ArrowBinding(_saveStartTrm, _saveEndPos);
        }
    }

    public Tween ReChainning(Action callBack, Entity entity)
    {
        MarkingEntity = entity;

        SetActive();
        float deltaY = _arrowMask.sizeDelta.y;
        Tween re = DOTween.To(() => new Vector2(0, deltaY), vec => _arrowMask.sizeDelta = vec, new Vector2(_saveLength, deltaY), 0.3f).SetEase(Ease.InQuart);
        re.OnComplete(() => 
        {
            FeedbackManager.Instance.ShakeScreen(0.5f);
            _chainArrowVisual.DOFade(0, 0.1f);
            SetFade(0f);
            
            callBack();
        });

        IsBindSucess = true;
        return re;
    }

    public void ArrowBinding(Transform startTrm, Vector2 endPos)
    {
        _chainArrowVisual.transform.position = MaestrOffice.GetWorldPosToScreenPos(Input.mousePosition);
        transform.localPosition = startTrm.localPosition;

        SetAngle((endPos - (Vector2)startTrm.localPosition).normalized);
        SetLength(startTrm.localPosition, endPos);

        _saveEndPos = endPos;
        _saveStartTrm = startTrm;
    }

    private void SetLength(Vector2 startPos, Vector2 endPos)
    {
        float distance = Mathf.Sqrt(Mathf.Pow(endPos.x - startPos.x, 2) + Mathf.Pow(endPos.y - startPos.y, 2));
        _arrowMask.sizeDelta = new Vector2(distance, _arrowMask.sizeDelta.y);
        _saveLength = distance;
    }

    private void SetAngle(Vector2 dir)
    {
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle -180, Vector3.forward);
        _arrowMask.localRotation = rotation;
        _chainArrowVisual.transform.localRotation = rotation;
    }
}
