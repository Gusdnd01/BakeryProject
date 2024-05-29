using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDetailedInfoPanel : PanelUI
{
    [SerializeField] private CanvasGroup _canvasGroup;
    private Tween _scalingTween;

    private AdventureData _adventureData = new AdventureData();
    private const string _adventureKey = "AdventureKEY";

    [Header("플레이어 정보")]
    [SerializeField] private TextMeshProUGUI _levelText;
    [SerializeField] private TextMeshProUGUI _nickNameText;

    [Header("진행도")]
    [SerializeField] private TextMeshProUGUI _advenPercent;
    [SerializeField] private TextMeshProUGUI _dungeonPercent;
    [SerializeField] private TextMeshProUGUI _mazePercent;

    [SerializeField] private Image _advenGaze;
    [SerializeField] private Image _dungeonGaze;
    [SerializeField] private Image _mazeGaze;

    private const int _advenMaxStage = 36;
    private const int _dungeonMax = 10;
    private const int _mazeMax = 10;

    [Header("가중치")]
    [SerializeField] private TextMeshProUGUI _atkAddvalueText;
    [SerializeField] private TextMeshProUGUI _defAddvalueText;
    [SerializeField] private TextMeshProUGUI _hpAddvalueText;

    public void EnablePanel()
    {
        _scalingTween?.Kill();

        _canvasGroup.transform.localScale = Vector3.one * 1.1f;
        _canvasGroup.alpha = 1f;
        FadePanel(true);

        _scalingTween = _canvasGroup.transform.DOScale(1, 0.1f);
    }
    public void DisablePanel()
    {
        _scalingTween?.Kill();

        _canvasGroup.transform.localScale = Vector3.one;
        FadePanel(false);

        Sequence seq = DOTween.Sequence();
        _scalingTween = seq;
        seq.Append(_canvasGroup.transform.DOScale(1.1f, 0.1f));
        seq.Join(_canvasGroup.DOFade(0, 0.1f));
    }

    public void SetPlayerData(PlayerData data)
    {
        int level = data.level;

        _levelText.text = $"<size=40><color=#4F2620>LV.</color></size>{level}";
        _nickNameText.text = data.nickName;

        _atkAddvalueText.text = 
        $"<color=#4F2620>ATK: </color>+{GetAddValue(level, 2)}% <color=#4F2620>(합산: {data.attak * GetAddValue(level, 2)})";

        _defAddvalueText.text =
        $"<color=#4F2620>DEF: </color>+{GetAddValue(level, 1)}% <color=#4F2620>(합산: {data.attak * GetAddValue(level, 1)})";

        _hpAddvalueText.text =
        $"<color=#4F2620>HP: </color>+{GetAddValue(level, 3)}% <color=#4F2620>(합산: {data.attak * GetAddValue(level, 3)})";
    }

    private int GetAddValue(int level, int value)
    {
        return level * value * Mathf.RoundToInt(Mathf.Pow(level - 1, 1.4f));
    }

    private void Start()
    {
        if (DataManager.Instance.IsHaveData(_adventureKey))
        {
            _adventureData = DataManager.Instance.LoadData<AdventureData>(_adventureKey);
        }

        string[] advenValue = _adventureData.InChallingingStageCount.Split('-');
        float advenCount = (Convert.ToInt16(advenValue[0]) - 1) * 6 + Convert.ToInt16(advenValue[1]);
        float dunCount = Convert.ToInt16(_adventureData.ChallingingMineFloor) - 1;
        float mazeCount = Convert.ToInt16(_adventureData.InChallingingMazeLoad) - 1;

        _advenGaze.fillAmount = advenCount / _advenMaxStage;
        _dungeonGaze.fillAmount = dunCount / _dungeonMax;
        _mazeGaze.fillAmount = mazeCount / _mazeMax;

        _advenPercent.text = $"{Mathf.FloorToInt((advenCount / _advenMaxStage) * 100)}%";
        _dungeonPercent.text = $"{Mathf.FloorToInt((dunCount / _dungeonMax) * 100)}%";
        _mazePercent.text = $"{Mathf.FloorToInt((mazeCount / _mazeMax) * 100)}%";
    }
}
