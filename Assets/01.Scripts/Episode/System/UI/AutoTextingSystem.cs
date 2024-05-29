using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoTextingSystem : MonoBehaviour
{
    private bool _isInAuto = false;

    private float _currentSecond;
    [SerializeField] private float _waitTime;

    private EpisodeManager _episodeManager;

    private void Start()
    {
        _episodeManager = EpisodeManager.Instanace;
    }

    public void ChangeAuto()
    {
        _isInAuto = !_isInAuto;
    }

    public void Update()
    {
        if (_episodeManager.isTextInTyping || !_isInAuto) return;

        if(_currentSecond >= _waitTime)
        {
            _currentSecond = 0;
            _episodeManager.NextDialogue();
            return;
        }
        _currentSecond += Time.deltaTime;
    }
}
