using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class DamagePopupText : MonoBehaviour
{
    [SerializeField] private TMP_Text _tmp;
    [SerializeField] private Color _normalColor;
    [SerializeField] private Color _criticalColor;

    public void PopUpDamage(int damage, Vector2 pos, bool isCritical)
    {
        Color textColor = isCritical ? _criticalColor : _normalColor;

        _tmp.color = textColor;
        _tmp.fontSize = isCritical ? 10 : 6;
        _tmp.text = damage.ToString();

        pos.y += 0.5f;
        Vector3 myPos =  pos;
        myPos.z = -5;
        transform.position = myPos;

        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(0.2f);
        seq.Append(_tmp.DOFade(0, 0.5f));
        seq.OnComplete(() => Destroy(gameObject));
    }
}
