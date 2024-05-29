using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UIDefine;
using System;

public class PanelManager : MonoSingleton<PanelManager>
{
    [SerializeField] private PanelUI[] _panelElementGroup;
    private Dictionary<PanelType, Stack<PanelUI>> _panelUIDic = new Dictionary<PanelType, Stack<PanelUI>>();

    private void Awake()
    {
        foreach (PanelUI pu in _panelElementGroup)
        {
            if (_panelUIDic.ContainsKey(pu.PanelType))
            {
                Debug.LogError("Error!");
                break;
            }

            _panelUIDic.Add(pu.PanelType, new Stack<PanelUI>());

            int toCreateCount = pu.canSeconderyActivePanel ? pu.toCreatedCount : 1;
            for (int i = 0; i < toCreateCount; i++)
            {
                PanelUI panelUIInstance = Instantiate(pu, transform);
                panelUIInstance.name = panelUIInstance.name.Replace("(Clone)", "");
                panelUIInstance.gameObject.SetActive(false);

                _panelUIDic[pu.PanelType].Push(panelUIInstance);
            }
        }
    }

    public PanelUI CreatePanel(PanelType panelType, Transform parent, Vector2 screenPos)
    {
        if (_panelUIDic[panelType].Count == 0)
        {
            Debug.LogError("Error! This Panel not allow secondery create");
            return null;
        }

        PanelUI pu = _panelUIDic[panelType].Pop();
        pu.transform.SetParent(parent);
        pu.transform.localPosition = screenPos;
        pu.transform.localScale = Vector3.one;

        return pu;
    }

    public void DeletePanel(PanelUI panelUI)
    {
        panelUI.transform.SetParent(transform);
        panelUI.gameObject.SetActive(false);

        _panelUIDic[panelUI.PanelType].Push(panelUI);
    }
}
