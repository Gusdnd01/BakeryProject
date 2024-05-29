using Cinemachine;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoSingleton<UIManager>
{
    [SerializeField] private Transform _uiParent;
    [SerializeField] private SceneUI[] _screenElementGroup;

    private Dictionary<SceneType, SceneUI> _sceneUIDic = new Dictionary<SceneType, SceneUI>();

    private SceneType CurrentSceneType => SceneObserver.CurrentSceneType;
    private SceneUI _currentSceneUIObject;

    public Canvas Canvas { get; private set; }
    public RectTransform CanvasTrm => Canvas.transform as RectTransform;

    public CinemachineBrain CinemachineBrain { get; private set; }
    public CinemachineVirtualCamera VirtualCamera { get; private set; }

    private void Start()
    {
        Canvas = GetComponentInChildren<Canvas>();
        CinemachineBrain = GetComponentInChildren<CinemachineBrain>();
        VirtualCamera = GetComponentInChildren<CinemachineVirtualCamera>(); 

        foreach(SceneUI su in _screenElementGroup)
        {
            _sceneUIDic.Add(su.ScreenType, su);
        }

        SceneManager.sceneLoaded += ChangeSceneUIOnChangeScene;
        ChangeSceneUIOnChangeScene(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= ChangeSceneUIOnChangeScene;
    }

    public T GetSceneUI<T>() where T : SceneUI
    {
        return (T)FindFirstObjectByType(typeof(T));
    }
    
    public void ChangeSceneUIOnChangeScene(Scene updateScene, LoadSceneMode mode)
    {
        if(_currentSceneUIObject != null)
        {
            _currentSceneUIObject.SceneUIEnd();
            Destroy(_currentSceneUIObject.gameObject);
        }

        if (_sceneUIDic.ContainsKey(CurrentSceneType))
        {
            SceneUI suObject = Instantiate(_sceneUIDic[CurrentSceneType], _uiParent);
            suObject.gameObject.name = _sceneUIDic[CurrentSceneType].gameObject.name + "_MAESTRO_[SceneUI]_";
            suObject.SceneUIStart();

            _currentSceneUIObject = suObject;
        }
    }
}
