using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CardSetter : MonoBehaviour
{
    public abstract void SetCardInfo(CardShameElementSO shameData, CardInfo cardInfo, int combineLevel);
}
