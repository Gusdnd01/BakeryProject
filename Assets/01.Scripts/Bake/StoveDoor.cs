using DG.Tweening;
using ExtensionFunction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveDoor : MonoBehaviour, IBakingProductionObject
{
    [SerializeField] private float _normalY;
    [SerializeField] private float _openY;
    [SerializeField] private float _easingTime;

    public float EasingTime { get; set; } = 0.3f;

    public void OnProduction()
    {
        transform.SmartMoveY(true, _openY, _easingTime);
    }

    public void ExitProduction()
    {
        transform.SmartMoveY(true, _normalY, _easingTime);
    }

    public void DoughInStove(int grade)
    {
        transform.SmartMoveY(true, _normalY, _easingTime * 3f, Ease.OutBounce);
    }

    public void OpenDoor()
    {
        transform.SmartMoveY(true, _openY, _easingTime * 1.5f, Ease.OutBounce);
    }
}
