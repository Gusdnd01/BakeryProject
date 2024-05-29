using DG.Tweening;
using UnityEngine;

public class CardProductionRecord
{
    public Tween OnPlayingTween { get; set; }

    private CardProductionType _onPlayingType;
    private Transform _onPlayingTrm;

    public CardProductionRecord(CardProductionType type, Transform trm)
    {
        _onPlayingType = type;
        _onPlayingTrm = trm;
    }

    public void Kill()
    {
        OnPlayingTween.Kill();
    }

    public bool IsSameType(CardProductionType type, Transform trm)
    {
        return (type == _onPlayingType) && (trm == _onPlayingTrm);
    }
}
