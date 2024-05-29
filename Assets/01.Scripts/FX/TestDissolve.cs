using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TestDissolve : MonoBehaviour
{
    [SerializeField] private ParticleSystem dissolveParticle;
    [SerializeField] private Image image;
    [SerializeField] private Shader baseShader;

    Material mat;

    private void Start()
    {
        mat = image.material;
        StartCoroutine(Dissolve());
    }

    private IEnumerator Dissolve()
    {
        dissolveParticle.Play();
        float dissolve = 1.0f;
        while (true)
        {
            yield return null;

            if (dissolve <= -1.0f)
            {
                dissolve = -1.0f;
                dissolveParticle.Stop();
                break;
            }

            dissolve -= 0.01f;
            mat.SetFloat("_dissolve_amount", dissolve);
        }
        mat.SetFloat("_dissolve_amount", 1.0f);
    }
}
