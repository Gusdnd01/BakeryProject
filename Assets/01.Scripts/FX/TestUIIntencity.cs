using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class TestUIIntencity : MonoBehaviour
{
    [SerializeField]
    [Range(0.0f, 10.0f)]
    private float _intencity;

    [SerializeField]
    private Color _startColor = new Color(191,51,183,255);

    [SerializeField]
    private Image _image;
    private Material _mat;

    private void Awake()
    {
        _intencity = 0.0f;
        _image = GetComponent<Image>();
        _mat = _image.material;

        _mat.SetColor("_outline_color", new Color(191, 41, 183, 255));
    }

    private void Update()
    {
        _mat.SetColor("_outline_color", _startColor *  _intencity);
    }
}
