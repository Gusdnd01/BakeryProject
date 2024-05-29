using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DissolveCard : MonoBehaviour
{
    [SerializeField]
    [Range(-1.0f, 1.0f)]
    private float dissolve_amount = 0.0f;

    [SerializeField]
    private bool _isDissolve;

    private Material mat;

    private void Start()
    {
        mat = GetComponent<UnityEngine.UI.Image>().material;
    }

    private void Update()
    {
        if (!_isDissolve) return;
        mat.SetFloat("_dissolve_amount", dissolve_amount);
    }
}
