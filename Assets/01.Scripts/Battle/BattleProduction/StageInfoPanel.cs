using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StageInfoPanel : PanelUI
{
    [SerializeField] private float _chaingValue;

    [Header("ÂüÁ¶°ª")]
    [SerializeField] private List<Sprite> _stageTypeIconList = new List<Sprite>();
    [SerializeField] private Image _stageIcon;
    [SerializeField] private TextMeshProUGUI _stageNameLabel;
    [SerializeField] private TextMeshProUGUI _clearConditionLabel;

    private Vector2 _stageIconNPos;
    private Vector2 _cleatConditionLabelNPos;

    public void SetInfo(StageDataSO stageData)
    {
        gameObject.SetActive(true);

        _stageNameLabel.text = stageData.stageName;
        _clearConditionLabel.text = $"Clear : {stageData.clearCondition.Info}";
        _stageIcon.sprite = _stageTypeIconList[(int)stageData.stageType];

        _stageIconNPos = _stageIcon.transform.localPosition;
        _cleatConditionLabelNPos = _clearConditionLabel.transform.localPosition;

        _stageIcon.transform.localPosition += new Vector3(0, _chaingValue, 0);
        _clearConditionLabel.transform.localPosition -= new Vector3(0, _chaingValue, 0);
    }

    public void PanelSetUp()
    {
        FadePanel(true);

        Sequence seq = DOTween.Sequence();

        seq.Append(
        _stageIcon.transform.DOLocalMoveY(_stageIconNPos.y - _chaingValue, 0.2f));
        seq.Join(
        _clearConditionLabel.transform.DOLocalMoveY(_cleatConditionLabelNPos.y + _chaingValue, 0.2f));
        seq.AppendInterval(1);
        seq.AppendCallback(() => FadePanel(false));
        seq.AppendInterval(0.4f);
        seq.AppendCallback(() => 
        {
            gameObject.SetActive(false);
            UIManager.Instance.GetSceneUI<BattleUI>().SystemActive?.Invoke(true);
        });
    }
}
