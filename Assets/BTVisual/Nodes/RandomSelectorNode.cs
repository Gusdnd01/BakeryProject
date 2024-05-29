using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BTVisual
{
    public class RandomSelectorNode : CompositeNode
    {
        private int _idx;
        protected override void OnStart()
        {
            _idx = Random.Range(0, children.Count);
        }

        protected override void OnStop()
        {
        }

        protected override State OnUpdate()
        {
            var child = children[_idx];

            return child.Update();
        }
    }
}
