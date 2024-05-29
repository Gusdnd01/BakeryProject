using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleResultProfilePanel : MonoBehaviour
{
    [SerializeField] private Image _profileImg;
    
    public void SetProfile(Sprite visual)
    {
        _profileImg.sprite = visual;

        Vector2 normalScale = transform.localScale;
        transform.localScale = normalScale * 1.15f;

        transform.DOScale(normalScale, 0.2f);
    }
}
