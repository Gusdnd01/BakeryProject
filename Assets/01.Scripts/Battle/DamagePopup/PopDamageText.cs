using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using Random = UnityEngine.Random;

public class PopDamageText : PoolableMono
{
    [SerializeField] private Vector3 _moveEndOffset;

    [SerializeField] private Vector2 _reactionMinOffset;
    [SerializeField] private Vector2 _reactionMaxOffset;
    [SerializeField] private GameObject _criticalFrame;
    [SerializeField] private TextMeshPro _damageText;
    public TextMeshPro DamageText => _damageText;

    public void ShowDamageText(Vector3 position, int damage, float fontSize, Color color)
    {
        _damageText.color = color;
        _damageText.fontSize = fontSize;
        _damageText.text = damage.ToString();

        position.z = -5;
        transform.position = position;
        Sequence seq = DOTween.Sequence();
        Vector3 endPos = Vector2.zero;

        if (position.x < 0)
        {
            endPos = transform.position - new Vector3(_moveEndOffset.x, -_moveEndOffset.y);
        }
        else
        {
            endPos = transform.position + _moveEndOffset;
        }

        seq.Append(transform.DOMove(endPos + GetRandomnessPos(), 0.5f).SetEase(Ease.OutQuart));
        seq.Append(_damageText.DOFade(0, 0.5f));
        seq.OnComplete(() => PoolManager.Instance.Push(this));
    }
    public void ShowReactionText(Vector3 position, string word, float fontSize, Color color)
    {
        Debug.Log(9);
        _damageText.color = color;
        _damageText.fontSize = fontSize;
        _damageText.text = word;

        position.z = -5;
        transform.position = position;

        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOMove(transform.position + new Vector3(0, 0.2f), 0.7f));
        seq.Join(_damageText.DOFade(0, 1f));
        seq.OnComplete(() => PoolManager.Instance.Push(this));
    }
    
    public void ActiveCriticalDamage()
    {
        _criticalFrame.SetActive(true);
    }
    private Vector3 GetRandomnessPos()
    {
        return new Vector2(Random.Range(_reactionMinOffset.x, _reactionMaxOffset.y),
                            Random.Range(_reactionMinOffset.y, _reactionMaxOffset.y));
    }

    public override void Init()
    {
        _damageText.color = Color.white;
        _criticalFrame.SetActive(false);
    }
}
