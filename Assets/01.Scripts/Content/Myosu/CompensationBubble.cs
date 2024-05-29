using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CompensationBubble : MonoBehaviour
{
    [SerializeField] private Image _visual;
    [SerializeField] private Image _comcompensationVisual;
    [SerializeField] private TextMeshProUGUI _comcompensationAmountTxt;

    private Vector2 _normalScale;

    private void Start()
    {
        _normalScale = transform.localScale;
        transform.localScale = Vector2.zero;

        Color c = _visual.color;
        c.a = 0;
        _visual.color = c;
    }

    public void SpeachUpBubble(Sprite compensationVisual, int amount)
    {
        _comcompensationVisual.sprite = compensationVisual;
        _comcompensationAmountTxt.text = $"X{amount}";

        transform.localScale = Vector3.zero;
        _visual.color = new Color(0, 0, 0, 0);

        transform.DOScale(_normalScale, 0.2f);
        _visual.DOFade(0.9f, 0.2f);
    }

    public void SpeachDownBubble()
    {
        transform.localScale = _normalScale;
        _visual.color = new Color(0, 0, 0, 0.9f);

        transform.DOScale(Vector3.zero, 0.2f);
        _visual.DOFade(0, 0.2f);
    }
}
