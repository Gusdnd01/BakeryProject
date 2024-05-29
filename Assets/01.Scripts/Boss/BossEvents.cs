using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMoveTargetEndEvent
{
    public void HandleEndMoveToTarget();
    public void HandleEndMoveToOriginPos();
}
