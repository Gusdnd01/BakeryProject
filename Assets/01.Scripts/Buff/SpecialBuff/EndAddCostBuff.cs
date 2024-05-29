using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndAddCostBuff : SpecialBuff
{
    public int addCostValue = 3;

    public override void Active(int level)
    {
        base.Active(level);
        CostCalculator.GetCost(addCostValue);
        SetIsComplete(true);
    }
}
