using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(CanvasGroup))]
public abstract class FuncBlock : MonoBehaviour
{
    [SerializeField] protected OptionGroup _optionGroup;
    [SerializeField] protected TextMeshProUGUI _notifyIsChangeText;

    protected bool _isHasChanges;
    protected bool IsHasChanges
    {
        get
        {
            return _isHasChanges;
        }
        set
        {
            _isHasChanges = value;
            _notifyIsChangeText.enabled = value;
        }
    }

    private CanvasGroup _canvasGroup;
    public CanvasGroup CanvasGroup
    {
        get
        {
            if (_canvasGroup != null) return _canvasGroup;

            _canvasGroup = GetComponent<CanvasGroup>();
            return _canvasGroup;
        }
    }
    
    public abstract void SaveData();
    public abstract void SetInitialValue();
}
