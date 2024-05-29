using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

enum ShakeTypeEnum
{
    Default,
    PositionWithVelocity
}

[RequireComponent(typeof(CinemachineImpulseSource))]
public class ShakeCamFeedback : MonoBehaviour, Feedback
{

    [SerializeField] ShakeTypeEnum shakeType;

    [SerializeField] private Rigidbody2D _rigid;
    [SerializeField] private float impulseRatio = 0.5f;
    private CinemachineImpulseSource impulseSource;

    private void Awake()
    {
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    public void CompleteFeedback()
    {
    }

    public void CreateFeedback()
    {
        switch (shakeType)
        {
            case ShakeTypeEnum.Default:
                impulseSource.GenerateImpulse(impulseRatio);
                break;
            case ShakeTypeEnum.PositionWithVelocity:
                impulseSource.GenerateImpulseAtPositionWithVelocity(transform.position, _rigid.velocity.normalized * impulseRatio * -1);
                break;
        }
    }
}
