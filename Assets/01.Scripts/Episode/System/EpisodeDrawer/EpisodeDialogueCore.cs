using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using EpisodeDialogueDefine;

public class EpisodeDialogueCore : MonoBehaviour
{
    [SerializeField] private EpisodeData _selectEpisodeData;
    private DialogueElement _selectDialogueElement;
    private EpisodeManager epiManager;

    [SerializeField] private UnityEvent<string, string, BackGroundType> StandardDrawEvent;
    [SerializeField] private UnityEvent<FadeOutType> ProductionDrawEvent;
    [SerializeField] private UnityEvent<bool, Sprite> PriviewImageDrawEvent;
    [SerializeField] private UnityEvent<CharacterType, FaceType, CharacterActiveType, bool> CharacterDrawEvent;
    [SerializeField] private UnityEvent<CharacterType, Vector2, Quaternion> CharacterMoveEvent;
    [SerializeField] private UnityEvent<CharacterType, EmotionType> CharacterEmotionEvent;

    private void Start()
    {
        HandleEpisodeStart(_selectEpisodeData);
    }
    public void HandleEpisodeStart(EpisodeData episodeData)
    {
        epiManager = EpisodeManager.Instanace;
        _selectEpisodeData = episodeData;
        HandleNextDialogue();
    }

    public void HandleNextDialogue()
    {
        if(epiManager.PauseIdx.Length > 0)
        {
            if (epiManager.DialogueIdx == epiManager.PauseIdx[epiManager.PuaseCount])
            {
                epiManager.SetPauseEpisode(true);
                epiManager.PuaseCount++;
                return;
            }
        }

        if (epiManager.DialogueIdx == _selectEpisodeData.dialogueElement.Count)
        {
            epiManager.EpisodeEndEvent?.Invoke();
            return;
        }
        
        _selectDialogueElement = _selectEpisodeData.dialogueElement[epiManager.DialogueIdx];
        PhaseEventConnect();

        if(_selectEpisodeData.dialogueElement.Count > epiManager.DialogueIdx)
        {
            while (_selectEpisodeData.dialogueElement[epiManager.DialogueIdx].isLinker)
            {
                _selectDialogueElement = _selectEpisodeData.dialogueElement[epiManager.DialogueIdx];
                PhaseConnectStandard();

                if(_selectEpisodeData.dialogueElement.Count == epiManager.DialogueIdx)
                {
                    break;
                }
            }
        }
    }

    private void PhaseEventConnect()
    {
        StandardDrawEvent?.Invoke(_selectDialogueElement.standardElement.name,
                                  _selectDialogueElement.standardElement.sentence,
                                  _selectDialogueElement.standardElement.backGroundType);

        PhaseConnectStandard();
    }

    private void PhaseConnectStandard()
    {
        CharacterType characterType = EpiswordMaster.GetCharacterTypeByName(_selectDialogueElement.standardElement.name);

        ProductionDrawEvent?.Invoke(_selectDialogueElement.productElement.fadeType);
        PriviewImageDrawEvent?.Invoke(_selectDialogueElement.priviewSprite != null, _selectDialogueElement.priviewSprite);

        epiManager.AddDialogeLogData(              characterType,
                                                   _selectDialogueElement.standardElement.name,
                                                   _selectDialogueElement.standardElement.sentence);

        CharacterDrawEvent?.Invoke(characterType,
                                   _selectDialogueElement.characterElement.faceType,
                                   _selectDialogueElement.captureElement.activeType,
                                   _selectDialogueElement.characterElement.isShake);

        CharacterMoveEvent?.Invoke(characterType,
                                   _selectDialogueElement.captureElement.movePosition,
                                   _selectDialogueElement.captureElement.rotationValue);

        CharacterEmotionEvent?.Invoke(characterType,
                                      _selectDialogueElement.characterElement.emotionType);

        
        epiManager.DialogueIdx++;
    }
}
