using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Content : MonoBehaviour
{
    [SerializeField] private SceneType _sceneType;
    public SceneType SceneType => _sceneType;

    public virtual void ContentStart()
    {

    }

    public virtual void ContentEnd() 
    { 
    }
}
