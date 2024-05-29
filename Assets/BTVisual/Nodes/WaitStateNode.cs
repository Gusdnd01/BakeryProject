using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BTVisual
{
    public class WaitStateNode : DecoratorNode
    {
        [SerializeField] private TurnStatus _chkState =TurnStatus.Running;
        protected override void OnStart()
        {
        }

        protected override void OnStop()
        {
        }

        protected override State OnUpdate()
        {
            if (brain.turnStatus != _chkState)
            {
                return State.FAILURE;
            }
            child.Update();
            return State.RUNNING;



        }
    }
}