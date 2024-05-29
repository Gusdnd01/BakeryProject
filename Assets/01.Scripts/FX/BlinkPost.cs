using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkPost : MonoBehaviour
{
    [Header(header:"blink settings")]
    [SerializeField] private bool _blinkStartIsBlack = false;
    [SerializeField] private int _blinkCount = 3;

    [Header(header: ("blink status"))]
    [SerializeField] private float _blinkFrameTime = 0.01f;

    private Material _mat;
    private bool _current = false;

    #region Coroutine Status
    private WaitForSeconds _blinkSeconds;
    #endregion

    private void Awake()
    {
        _mat = GetComponent<SpriteRenderer>().material;
        if(_blinkStartIsBlack) _mat.SetInt("_use_one_minus", 0);
        else _mat.SetInt("_use_one_minus", 1);

        _blinkSeconds = new WaitForSeconds(_blinkFrameTime);

    }

    [ContextMenu(itemName:"BlinkEffect")]
    public void Blink()
    {
        gameObject.SetActive(true);
        StartCoroutine(BlinkCo());
    }

    private IEnumerator BlinkCo()
    {
        int temp = 0;
        for (int i = 0; i < _blinkCount; ++i)
        {
            temp = _current ? 1 : 0;
            _current = !_current;
            yield return _blinkSeconds;
            _mat.SetInt("_use_one_minus", temp);
        }

        yield return _blinkSeconds;
        gameObject.SetActive(false);
    }
}
