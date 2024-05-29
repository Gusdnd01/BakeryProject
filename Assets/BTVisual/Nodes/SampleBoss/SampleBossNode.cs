using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BTVisual
{
    public abstract class SampleBossNode : ActionNode
    {
        [SerializeField] private string parametorName;
        protected int animationHash;


        protected virtual void OnEnable()
        {
            animationHash = Animator.StringToHash(parametorName);
        }

        protected override void OnStart()
        {
            brain.AnimatorCompo.SetBool(animationHash, true);
        }

        protected override void OnStop()
        {
            brain.AnimatorCompo.SetBool(animationHash, false);
        }
    }
}