using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoSingleton<TimeManager>
{
    private Coroutine _delayCor;
    private float _baseTimeScale;

    public void SetTImeScale(float value)
    {
        Time.timeScale = value;
    }
    public void TimeScaleCor(float scaleValue, float time)
    {
        if (_delayCor == null)
        {
            _baseTimeScale = Time.timeScale;
            _delayCor = StartCoroutine(DelayCorutine(scaleValue, time));
        }
        else
        {
            StopCoroutine(_delayCor);
            _delayCor = StartCoroutine(DelayCorutine(scaleValue, time));
        }
    }
    private IEnumerator DelayCorutine(float value, float time)
    {
        Time.timeScale = value;
        yield return new WaitForSecondsRealtime(time);
        _delayCor = null;
        Time.timeScale = _baseTimeScale;
    }
}
