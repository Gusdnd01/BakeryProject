using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PreviewPanel : MonoBehaviour
{
    [SerializeField] private RecipeSortType _mySortType;
    public RecipeSortType MySortType => _mySortType;
    [SerializeField] private GameObject _myContent;

    public void SetUpPanel(RecipeSortType sortType)
    {
        bool isMatch = _mySortType == sortType;

        _myContent.gameObject.SetActive(isMatch);

        if(isMatch)
        {
            LookUpContent();
        }
    }

    protected abstract void LookUpContent();
}
