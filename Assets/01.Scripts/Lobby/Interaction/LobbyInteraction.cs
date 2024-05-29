using EpisodeDialogueDefine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyInteraction : MonoBehaviour
{
    private Dictionary<CharacterType, CharacterStand> _characterDic = new ();
    private List<EpisodeData> _interactionData = new ();

    private void Start()
    {
        CharacterStand[] cs = GetComponentsInChildren<CharacterStand>();
        foreach (CharacterStand c in cs)
        {
            _characterDic.Add(c.CharacterType, c);
            c.InterAction = this;
        }
    }

    private void OnDisable()
    {
        _characterDic.Clear();
    }

    public void StartInteraction()
    {
        int rdIdx = Random.Range(0, _interactionData.Count);
        EpisodeData episodeData = _interactionData[rdIdx];

        //StartCoroutine(InteractionSequence(episodeData));
    }

    //IEnumerator InteractionSequence(EpisodeData episodeData)
    //{
    //    for(int i = 0; i < episodeData.dialogueElement.Count; i++)
    //    {
    //        Character
    //    }
    //}
}
