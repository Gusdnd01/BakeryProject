using Particle.Trigger;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMoveParticle : ParticleTriggerEventBase
{
    public float orthoSize;

    private PoolVCam cam;
    public override void Action(ref ParticleSystem.Particle p, Collider2D col)
    {
        //if (cam == null || PoolManager.Instance.Contains(cam))
        //{
        //    Vector3 initPos = transform.TransformPoint(p.position);
        //    initPos.z = -10;
        //    cam.SetCamera(initPos, orthoSize);
        //    return;
        //}
        //Vector3 backwardPos = transform.TransformPoint(p.position);
        //backwardPos.z = -10;
        //cam.transform.position = backwardPos;
    }
}
