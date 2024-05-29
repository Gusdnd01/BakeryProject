using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RetryTrigger : MonoBehaviour
{
    [SerializeField] private Transform _goBackTrm;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        collision.transform.position = _goBackTrm.position;
    }
}
