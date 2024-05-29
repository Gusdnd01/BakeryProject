using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpeachBubble : MonoBehaviour
{
    [SerializeField] private Image _bubble;
    [SerializeField] private TextMeshProUGUI _text;
    private Sequence seq;

    public void SetLine(string line)
    {
        seq.Kill();
        _bubble.transform.localScale = Vector3.zero;
        _bubble.color = new Color(1, 1, 1, 0);
        _text.color = new Color(0, 0, 0, 0);

        seq = DOTween.Sequence();

        seq.Append(_bubble.transform.DOScale(Vector3.one, 0.2f));
        seq.Join(_bubble.DOFade(1, 0.2f));
        seq.Join(_text.DOFade(1, 0.2f));
        seq.InsertCallback(0, ()=> _text.text = line);
        seq.AppendInterval(1.5f);
        seq.Append(_bubble.DOFade(0, 0.2f));
        seq.Append(_text.DOFade(0, 0.2f));
    }
}
