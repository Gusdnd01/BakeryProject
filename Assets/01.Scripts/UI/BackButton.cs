using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackButton : MonoBehaviour
{
    public void Goback()
    {
        GameManager.Instance.ChangeScene(SceneObserver.BeforeSceneType);
    }
}
