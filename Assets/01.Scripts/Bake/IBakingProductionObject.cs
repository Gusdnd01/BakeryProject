using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBakingProductionObject
{
    public float EasingTime { get; set; }
    public abstract void OnProduction();
    public abstract void ExitProduction();
    public abstract void DoughInStove(int grade);
}
