using EpisodeDialogueDefine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EpiswordMaster
{
    public static CharacterType GetCharacterTypeByName(string name)
    {
        switch (name)
        {
            case "타르트":
                return CharacterType.tart;
            case "???":
                return CharacterType.mawang;
            case "크림":
                return CharacterType.mawang;
            case "크림슨 브륄레":
                return CharacterType.mawang;
            default:
                return CharacterType.tart;
        }
    }

    public static void SetEmotionReactionPos(Transform emoTrm, CharacterStandard characterStandard)
    {
        Transform selectEmoTrm;
        if(characterStandard.transform.localPosition.x >= 0)
        {
            selectEmoTrm = characterStandard.EmotionLeftPos;
        }
        else
        {
            selectEmoTrm = characterStandard.EmotionRightPos;
            selectEmoTrm.transform.localRotation = Quaternion.Euler(0, 180, 0);
        }

        emoTrm.SetParent(selectEmoTrm, false);
        emoTrm.localScale = Vector3.one * 3;
        emoTrm.transform.localPosition = Vector3.zero;
    }
}
