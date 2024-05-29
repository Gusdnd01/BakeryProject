using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using EpisodeDialogueDefine;
using System;

public struct LogData
{
    public Sprite characterSprite;
    public string characterName;
    public string characterSyntex;

    public LogData(Sprite img, string name, string syntex)
    {
        characterSprite = img;
        characterName = name;
        characterSyntex = syntex;
    }
}

public class EpisodeManager : MonoBehaviour
{
    private static EpisodeManager _instance;
    public static EpisodeManager Instanace
    {
        get
        {
            if (_instance != null) return _instance;
            _instance = FindObjectOfType<EpisodeManager>();
            if (_instance == null)
            {
                Debug.LogError("Not Exist EpisodeManager");
            }
            return _instance;
        }
    }

    [SerializeField] private GameObject _episodeGroup;
    [SerializeField] private UnityEvent<EpisodeData> _episodeStartEvent;
    [SerializeField] private UnityEvent _nextDialogueEvent;
    [SerializeField] private UnityEvent<bool> _activeSyntexPanel;
    [SerializeField] private Sprite[] _characterSprite;

    public Action EpisodeEndEvent { get; set; }

    [HideInInspector] public int DialogueIdx { get; set; }
    [HideInInspector] public List<LogData> dialogueLog = new List<LogData>();
    [HideInInspector] public bool isTextInTyping;

    [HideInInspector] public int PuaseCount { get; set; }
    private int[] _pauseIdx = new int[0];
    [HideInInspector] public int[] PauseIdx => _pauseIdx;
    private bool _isInPuase;
    public bool isInPause => _isInPuase;

    public void SetPauseEpisode(bool isPause)
    {
        ActiveUIPlanel(!isPause);
        _isInPuase = isPause;
    }

    public void SetPauseEpisode(bool isPause, Action callback)
    {
        ActiveUIPlanel(!isPause);
        _isInPuase = isPause;
        callback?.Invoke();
    }

    public void StartEpisode(EpisodeData data)
    {
        ActiveUIPlanel(true);
        _episodeStartEvent?.Invoke(data);
    }

    public void StartEpisode(EpisodeData data, int[] pauseIndexs)
    {
        ActiveUIPlanel(true);
        _pauseIdx = pauseIndexs;
        _episodeStartEvent?.Invoke(data);
    }

    public void ActiveUIPlanel(bool isActive)
    {
        _episodeGroup.SetActive(isActive);
    }

    public void AddDialogeLogData(CharacterType ct, string name, string syntex)
    {
        LogData logData = new LogData(_characterSprite[(int)ct], name, syntex);
        dialogueLog.Add(logData);
    }

    public void NextDialogue()
    {
        _nextDialogueEvent?.Invoke();
    }

    public void ActiveSyntexPanel(bool isActive)
    {
        _activeSyntexPanel?.Invoke(isActive);
    }
}
