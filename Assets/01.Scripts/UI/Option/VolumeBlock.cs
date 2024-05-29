using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class VolumeBlock : FuncBlock
{
    [Header("참조")]
    [SerializeField] private Slider _masterSlider;
    [SerializeField] private Slider _bgmSlider;
    [SerializeField] private Slider _sfxSlider;

    private SoundData _soundData = new SoundData();
    private const string VolumeSettingKey = "VolumeDataKey";

    private float _savingMasterVolume;
    private float _savingBGMVolume;
    private float _savingSFXVolume;

    private void Awake()
    {
        _masterSlider.onValueChanged.AddListener(SetMasterVolumeValue);
        _bgmSlider.onValueChanged.AddListener(SetBgmVolumeValue);
        _sfxSlider.onValueChanged.AddListener(SetSfxVolumeValue);
    }

    private void OnEnable()
    {
        if(DataManager.Instance.IsHaveData(VolumeSettingKey))
        {
            _soundData = DataManager.Instance.LoadData<SoundData>(VolumeSettingKey);
        }

        _masterSlider.value = _soundData.MasterVoume;
        _bgmSlider.value = _soundData.BgmVolume;
        _sfxSlider.value = _soundData.SfxVolume;

        _savingMasterVolume = _soundData.MasterVoume;
        _savingBGMVolume = _soundData.BgmVolume;
        _savingSFXVolume = _soundData.SfxVolume;

        IsHasChanges = false;
    }

    #region 밸류 셋 메서드 개노답 삼인방
    private void SetMasterVolumeValue(float value)
    {
        _soundData.MasterVoume = value;
        IsHasChanges = true;
    }

    private void SetBgmVolumeValue(float value)
    {
        _soundData.BgmVolume = value;
        IsHasChanges = true;
    }

    private void SetSfxVolumeValue(float value)
    {
        _soundData.SfxVolume = value;
        IsHasChanges = true;
    }
    #endregion

    public override void SaveData()
    {
        _optionGroup.saveBtn.SaveData(_soundData, VolumeSettingKey, out _isHasChanges);
        _notifyIsChangeText.enabled = _isHasChanges;
    }

    public override void SetInitialValue()
    {
        _optionGroup.setInitialBtn.InitializeData(_soundData, out _isHasChanges);
        _notifyIsChangeText.enabled = _isHasChanges;
    }

    private void OnDisable()
    {
        if(IsHasChanges)
        {
            _soundData.MasterVoume = _savingMasterVolume;
            _soundData.BgmVolume = _savingBGMVolume;
            _soundData.SfxVolume = _savingSFXVolume;
        }
    }
}
