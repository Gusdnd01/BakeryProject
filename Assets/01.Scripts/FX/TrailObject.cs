using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailObject : MonoBehaviour
{
    private ParticleSystem self;

    private void Awake()
    {
        self = GetComponent<ParticleSystem>();  
    }

    private void LateUpdate()
    {
        transform.position = MaestrOffice.GetWorldPosToScreenPos(Input.mousePosition);
    }
}
