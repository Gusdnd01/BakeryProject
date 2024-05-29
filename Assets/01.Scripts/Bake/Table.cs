using DG.Tweening;
using ExtensionFunction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table : MonoBehaviour, IBakingProductionObject
{
    [SerializeField] private Vector2 _normalPos;
    [SerializeField] private Vector2 _disAppearPos;
    public float EasingTime { get; set; } = 0.3f;

    public void OnProduction()
    {
        transform.SmartMove(false, _disAppearPos, EasingTime);
    }
    public void ExitProduction()
    {
        transform.SmartMove(false, _normalPos, EasingTime);
    }

    public void DoughInStove(int grade)
    {

    }
}
