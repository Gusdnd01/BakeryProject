using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellCardManagement : CardManagement
{
    public override void UseAbility(CardBase selectCard)
    {
        selectCard.Abillity();
    }
}
