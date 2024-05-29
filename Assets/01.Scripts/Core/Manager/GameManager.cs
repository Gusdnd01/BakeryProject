using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoSingleton<GameManager>
{
    [Header("Contents")]
    [SerializeField] private List<Content> _contentList = new List<Content>();
    private Dictionary<SceneType, Content> _contentDic = new Dictionary<SceneType, Content>();
    private SceneType CurrentSceneType => SceneObserver.CurrentSceneType;
    private Content _currentContent;

    [Header("Pooling")]
    [SerializeField] private PoolListSO _poolingList;
    [SerializeField] private Transform _poolingTrm;

    [Header("Fade")]
    [SerializeField] private FadePanel _fadePanel;

    private void Start()
    {
        foreach(Content content in _contentList)
        {
            if(_contentDic.ContainsKey(content.SceneType))
            {
                Debug.LogError($"Error : {content.SceneType} has overlap!!");
                continue;
            }

            _contentDic.Add(content.SceneType, content);
        }

        SceneManager.sceneLoaded += ChangeSceneContentOnChangeScene;
        ChangeSceneContentOnChangeScene(SceneManager.GetActiveScene(), LoadSceneMode.Single);
        PoolSetUp();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= ChangeSceneContentOnChangeScene;
    }

    public T GetContent<T>() where T : Content
    {
        return (T)FindFirstObjectByType(typeof(T));
    }
    private void ChangeSceneContentOnChangeScene(Scene updateScene, LoadSceneMode mode)
    {
        if (_currentContent != null)
        {
            _currentContent.ContentEnd();
            Destroy(_currentContent.gameObject);
        }

        if (_contentDic.ContainsKey(CurrentSceneType))
        {
            Content contentObj = Instantiate(_contentDic[CurrentSceneType]);
            contentObj.gameObject.name = _contentDic[CurrentSceneType].gameObject.name + "_MAESTRO_[Content]_";
            contentObj.ContentStart();

            _currentContent = contentObj;
        }
    }
    private void PoolSetUp()
    {
        PoolManager.Instance = new PoolManager(_poolingTrm);
        foreach (PoolingItem item in _poolingList.poolList)
        {
            PoolManager.Instance.CreatePool(item.prefab, item.type, item.count);
        }
    }
    public void ChangeScene(SceneType toChangingScene)
    {
        SceneObserver.BeforeSceneType = CurrentSceneType;

        StartCoroutine(Fade(toChangingScene));

        //SceneObserver.CurrentSceneType = SceneType.loading;
        //SceneManager.LoadScene("LoadingScene");
        //StartCoroutine(LoadingProcessCo(toChangingScene));
    }
    
/*    private IEnumerator LoadingProcessCo(SceneType toChangingSceneType)
    {
        yield return null;

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync("ActiveScene");
        asyncOperation.allowSceneActivation = false;

        while (!asyncOperation.isDone)
        {
            LoadingProgress = Mathf.CeilToInt(asyncOperation.progress * 100);
            if (asyncOperation.progress >= 0.9f)
            {
                yield return new WaitForSeconds(2.0f);
                yield return _fadePanel.StartFade(Vector2.zero);
                SceneObserver.CurrentSceneType = toChangingSceneType;
                asyncOperation.allowSceneActivation = true;
            }
            yield return null;
        }
    }

        SceneObserver.CurrentSceneType = SceneType.loading;
        SceneObserver.CurrentSceneType = toChangingScene;
        SceneManager.LoadScene("ActiveScene");
    }*/
    
    public Scene GetCurrentSceneInfo()
    {
        return SceneManager.GetActiveScene();
    }

    private IEnumerator Fade(SceneType toChangingScene)
    {
        yield return _fadePanel.StartFade(MaestrOffice.GetWorldPosToScreenPos(Input.mousePosition));

        SceneObserver.CurrentSceneType = SceneType.loading;
        SceneObserver.CurrentSceneType = toChangingScene;
        SceneManager.LoadScene("ActiveScene");
    }
}