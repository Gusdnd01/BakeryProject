using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class InGameSettingBlock : FuncBlock
{
    [Header("참조")]
    
    [SerializeField] private TMP_InputField _vibrationValueField;
    [SerializeField] private Dropdown _screenModeDropDown;
    [SerializeField] private CheckBox _verticalSyncCheckBox;

    private InGameSettingData _inGameSettingData = new InGameSettingData();
    private Regex _numberFilter = new Regex(@"^[0-9]+$");

    private const string InGameSettingDatakey = "InGameDataKey";

    private void Awake()
    {
        _vibrationValueField.onValueChanged.AddListener(ChangeVibrationValue);
        _screenModeDropDown.OnValueChanged += ChangeModeType;
        _verticalSyncCheckBox.OnValueChanged += HandleGetVsyncValue;
    }

    public void Start()
    {
        if(DataManager.Instance.IsHaveData(InGameSettingDatakey))
        {
            _inGameSettingData = DataManager.Instance.LoadData<InGameSettingData>(InGameSettingDatakey);
        }

        _vibrationValueField.text = _inGameSettingData.vibrationValue.ToString();
        _screenModeDropDown.SetItem(_inGameSettingData.modeNum);
        _verticalSyncCheckBox.IsActive = _inGameSettingData.isVerticalSync;

    }

    private void ChangeVibrationValue(string sentencec)
    {
        sentencec = sentencec.Trim();
        if(!_numberFilter.IsMatch(sentencec))
        {
            // 숫자 아닌거 섞임
            return;
        }

        int value = Convert.ToInt32(sentencec);

        if(value < 0 || value > 100)
        {
            // 값 벗어남
            return;
        }

        _inGameSettingData.vibrationValue = value;
        IsHasChanges = true;
    }
    private void ChangeModeType(int num)
    {
        switch (num)
        {
            case 0:
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                break;
            case 1:
                Screen.fullScreenMode = FullScreenMode.Windowed;
                break;
            default:
                break;
        }

        _inGameSettingData.modeNum = num;
        IsHasChanges = true;
    }
    private void HandleGetVsyncValue (bool value)
    {
        QualitySettings.vSyncCount = value ? 1 : 0;
        _inGameSettingData.isVerticalSync = value;
        IsHasChanges = true;
    }

    public override void SaveData()
    {
        _optionGroup.saveBtn.SaveData(_inGameSettingData, InGameSettingDatakey, out _isHasChanges);
        _notifyIsChangeText.enabled = _isHasChanges;
    }

    public override void SetInitialValue()
    {
        _optionGroup.setInitialBtn.InitializeData(_inGameSettingData, out _isHasChanges);
        _notifyIsChangeText.enabled = _isHasChanges;
    }
}
