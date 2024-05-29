using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeedbackPlayer : MonoBehaviour
{
    private List<Feedback> _feedbackList = new List<Feedback>();

    private void Awake()
    {
        GetComponents<Feedback>(_feedbackList);
    }

    public void PlayFeedback()
    {
        foreach (Feedback feedback in _feedbackList)
        {
            feedback.CompleteFeedback();
            feedback.CreateFeedback();
        }
    }
}
