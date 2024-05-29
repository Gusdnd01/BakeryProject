using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAnimationEventHandler
{
    public void OnAnimationEventHandle();
}
public interface IAnimationEndHandler
{
    public void OnAnimationEndHandle();
}