using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShatterEffect : MonoBehaviour
{
    [SerializeField]
    private GameObject shatterEffect;

    [SerializeField]
    private float _explosionPower, _explosionRad;
    private Rigidbody[] rbs;

    private void OnEnable()
    {
        StartCoroutine(Shatter());
    }
    private IEnumerator Shatter()
    {
        yield return new WaitForSeconds(5.0f);
        GameObject obj = Instantiate(shatterEffect, Vector3.zero, Quaternion.identity);
        rbs = obj.transform.GetComponentsInChildren<Rigidbody>();

        foreach (var rb in rbs)
        {
            rb.AddExplosionForce(_explosionPower, transform.position + Vector3.forward, _explosionRad);
        }

        Destroy(obj, 2.0f);
    }
}
