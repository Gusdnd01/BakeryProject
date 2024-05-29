using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EpisodeSelecter : MonoBehaviour
{
    [SerializeField] private UnityEvent<EpisodeData> _episodeStartEvent;

    public void EpisodeStart(EpisodeData data)
    {
        _episodeStartEvent?.Invoke(data);
    }
}
