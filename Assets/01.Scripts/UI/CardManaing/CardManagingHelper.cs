using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class CardManagingHelper
{
    public static int GetCardShame(CardShameElementSO shameData, CardShameType type, int combineLevel)
    {
        if(shameData == null)
        {
            return 0;
        }

        return shameData.cardShameDataList[combineLevel].list[shameData.cardLevel - 1].list.FirstOrDefault(x => (x.cardShameType & type) != 0).currentShame;
    }

    public static int GetAfterLevelShame(CardShameElementSO shameData, CardShameType type, int combineLevel)
    {
        if (shameData.cardLevel >= 5) return GetCardShame(shameData, type, combineLevel);

        return shameData.cardShameDataList[combineLevel].list[shameData.cardLevel].list.FirstOrDefault(x => (x.cardShameType & type) != 0).currentShame;
    }

    public static CardShameData GetCardShameData(CardShameElementSO shameData, CardShameType type, int combineLevel)
    {
        return shameData.cardShameDataList[combineLevel].list[shameData.cardLevel].list.FirstOrDefault(x => x.cardShameType == type);
    }
}
