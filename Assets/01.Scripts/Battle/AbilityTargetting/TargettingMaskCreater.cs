using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargettingMaskCreater : MonoBehaviour
{
    private Dictionary<Enemy, TargetMask> _getTargetMaskDic = new Dictionary<Enemy, TargetMask>();  
    public Dictionary<Enemy, TargetMask> GetTargetMaskDic => _getTargetMaskDic;
    [SerializeField] private TargetMask _targetMaskRect;

    public void CreateMask(Enemy enemy, Vector2 maskPos)
    {
        Vector2 screenPoint = MaestrOffice.GetScreenPosToWorldPos(maskPos);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(UIManager.Instance.CanvasTrm,
            screenPoint, UIManager.Instance.Canvas.worldCamera, out Vector2 anchoredPosition);

        TargetMask tm = Instantiate(_targetMaskRect, transform);
        RectTransform rt = tm.transform as RectTransform;
        rt.anchoredPosition = anchoredPosition;

        _getTargetMaskDic.Add(enemy, tm);
    }

    public void MaskDown(Enemy enemy)
    {
        GetTargetMaskDic[enemy].gameObject.SetActive(false);
    }

    public void MaskUp(Enemy enemy)
    {
        GetTargetMaskDic[enemy].gameObject.SetActive(true);
    }
}
