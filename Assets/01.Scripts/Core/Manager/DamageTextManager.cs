using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;
using TMPro;

[Serializable]
public enum DamageCategory
{
    Noraml = 0,
    Critical = 1,
    Heal = 2,
    Debuff = 3,
}

[Serializable]
public struct ReactionWord
{
    public string[] reactionWordArr;
}

public class DamageTextManager : MonoSingleton<DamageTextManager>
{
    public bool _popupDamageText;

    [Header("Font")]
    [SerializeField] private TMP_FontAsset _damageTextFont;
    [SerializeField] private TMP_FontAsset _reactionTextFont;

    [Header("normal, critical, heal, debuff")]
    [ColorUsage(true, true)]
    [SerializeField] private Color[] _textColors;
    [SerializeField] private float[] _textSizes;
    [SerializeField] private ReactionWord[] _reactionType;

    public void PopupDamageText(Vector3 position, int number, DamageCategory category)
    {
        if (!_popupDamageText) return; //텍스트가 뜨기로 되어 있을 때만 띄운다.

        PopDamageText _damageText = PoolManager.Instance.Pop(PoolingType.DamageText) as PopDamageText;
        _damageText.DamageText.font = _damageTextFont;

        int idx = (int)category;
        _damageText.ShowDamageText(position, number, _textSizes[idx], _textColors[idx]);
        if(category == DamageCategory.Critical)
        {
            _damageText.ActiveCriticalDamage();
        }
    }

    public void PopupReactionText(Vector3 position, DamageCategory category)
    {
        PopDamageText _reactionText = PoolManager.Instance.Pop(PoolingType.DamageText) as PopDamageText;
        _reactionText.DamageText.font = _reactionTextFont;
        int idx = (int)category;

        int randomIdx = UnityEngine.Random.Range(0, _reactionType[idx].reactionWordArr.Length);
        _reactionText.ShowReactionText(position, _reactionType[idx].reactionWordArr[randomIdx], 10, _textColors[idx]);
    }

    public void PopupReactionText(Vector3 position, Color color, string message)
    {
        PopDamageText _reactionText = PoolManager.Instance.Pop(PoolingType.DamageText) as PopDamageText;
        _reactionText.DamageText.font = _reactionTextFont;
        _reactionText.ShowReactionText(position, message, 5, color);
    }
}
              