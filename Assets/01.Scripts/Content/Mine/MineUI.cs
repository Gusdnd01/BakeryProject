using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MineUI : BattleUI
{
    [SerializeField] private Animator _animator;
    public Animator StagePanelAnimator => _animator;
    private readonly int _setUpHash = Animator.StringToHash("isSetUp");

    [SerializeField] private TextMeshProUGUI _stageFloor;
    [SerializeField] private TextMeshProUGUI _stageName;
    [SerializeField] private TextMeshProUGUI _clearGemCount;
    [SerializeField] private GameObject _onlyFirstGemObj;

    private string _currentFloor;
    private string _currentStageName;
    private string _currentClearGem;
    private bool _isClearCurrentStage;

    private MineSystem _mineSystem;
    public MineSystem MineSystem
    {
        get
        {
            if (_mineSystem != null) return _mineSystem;
            _mineSystem = FindObjectOfType<MineSystem>();
            return _mineSystem;
        }
    }

    public override void SceneUIStart()
    {
        SceneObserver.BeforeSceneType = SceneType.Lobby;
    }

    public void SetFloor(string floor, string stageName, string clearGem, bool isClear)
    {
        _currentFloor = floor;
        _currentStageName = stageName;
        _currentClearGem = clearGem;
        _isClearCurrentStage = isClear;
    }

    public void PanelActive(bool isActive)
    {
        _animator.SetBool(_setUpHash, isActive);
    }

    public void SetUpFloor()
    {
        _stageFloor.text = _currentFloor;
        _stageName.text = _currentStageName;
        _clearGemCount.text = _currentClearGem;
        _onlyFirstGemObj.SetActive(!_isClearCurrentStage);
    }
}
