using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class PlayerHPUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _hpText;
    [SerializeField] private Slider _hpBar;
    [SerializeField] private Slider _hpBarTurm;
    private Sequence _playerGetDamageSequence;
    [SerializeField] private float _easingTime = 0.2f;

    public void SetHpOnUI(float playerCurrentHpValue, float playerMaxHpValue)
    {
        Debug.Log($"{playerCurrentHpValue}, {playerMaxHpValue}");

        _hpText.SetText($"{playerCurrentHpValue} / {playerMaxHpValue}");
        _hpText.transform.DOShakeRotation(_easingTime, 50, 30);

        float targetHpValue = playerCurrentHpValue / playerMaxHpValue;
        _playerGetDamageSequence = DOTween.Sequence();
        _playerGetDamageSequence.Append
        (
            DOTween.To(() => _hpBar.value, v => _hpBar.value = v, 
            targetHpValue, _easingTime)
        );
        _playerGetDamageSequence.Join
        (
            DOTween.To(() => _hpBarTurm.value, v => _hpBarTurm.value = v,
            targetHpValue, _easingTime + 0.15f)
        );
    }
}
