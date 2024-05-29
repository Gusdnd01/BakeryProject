using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum AdventureType
{
    Mission,
    Mine,
    Stage
}

public class AdventurePanel : MonoBehaviour
{
    [SerializeField] private AdventureType _adventureType;
    [SerializeField] private float _unSelectedValue;
    
    public void GoAdventure()
    {
        switch (_adventureType)
        {
            case AdventureType.Mission:
                GameManager.Instance.ChangeScene(SceneType.Myosu);
                break;
            case AdventureType.Mine:
                GameManager.Instance.ChangeScene(SceneType.Mine);
                break;
            case AdventureType.Stage:
                GameManager.Instance.ChangeScene(SceneType.mapSelect);
                break;
        }
    }
}
