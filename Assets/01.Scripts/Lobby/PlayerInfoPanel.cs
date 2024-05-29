using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlayerInfoPanel : MonoBehaviour
{
    private PlayerData _playerData = new PlayerData();

    [SerializeField] private TextMeshProUGUI _levelText;
    [SerializeField] private TextMeshProUGUI _nickNameText;

    [SerializeField] private Slider _expSlider;
    [SerializeField] private TextMeshProUGUI _expText;

    [SerializeField] private UnityEvent<PlayerData> _playerInfoSetEvent;

    private void Start()
    {
        if(DataManager.Instance.IsHaveData(DataKeyList.playerInfoDataKey))
        {
            _playerData = DataManager.Instance.LoadData<PlayerData>(DataKeyList.playerInfoDataKey);
        }

        _levelText.text = $"<size=30><color=#4F2620>LV.</color></size> \r\n{_playerData.level}";
        _nickNameText.text = _playerData.nickName;

        int maxExp = GetMaxExp(_playerData.level);

        _expSlider.value = _playerData.exp / maxExp;
        _expText.text = $"{_playerData.exp} / {maxExp}";

        _playerInfoSetEvent?.Invoke(_playerData);
    }

    private int GetMaxExp(int level)
    {
        return level * 10 * Mathf.RoundToInt(Mathf.Pow(level, 2));
    }
}
