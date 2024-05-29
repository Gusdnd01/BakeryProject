using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeScaleFeedback : MonoBehaviour, Feedback
{
    [SerializeField] private float _timeScale;
    [SerializeField] private float _restoreDelay;

    public void CompleteFeedback()
    {
    }

    public void CreateFeedback()
    {
        TimeManager.Instance.TimeScaleCor(_timeScale, _restoreDelay);
    }
}
