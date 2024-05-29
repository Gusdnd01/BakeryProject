using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UITouchEffect : MonoBehaviour
{
    private EffectObject effectObject;

    [SerializeField]
    private ParticleSystem mouseTrail;

    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            PoolableMono pam = PoolManager.Instance.Pop(PoolingType.TouchEffect);
            pam.transform.position = MaestrOffice.GetWorldPosToScreenPos(Input.mousePosition);
            mouseTrail.Play();
        }
        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            mouseTrail.Stop();
        }
    }
}
