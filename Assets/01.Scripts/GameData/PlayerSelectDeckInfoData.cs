using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSelectDeckInfoData : CanSaveData
{
    public string deckName;
    public List<string> PlayerSelectDeck = new List<string>();

    public override void SetInitialValue()
    {

    }
}
