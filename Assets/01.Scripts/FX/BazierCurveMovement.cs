using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BazierCurveMovement : MonoBehaviour
{
    [SerializeField] private Transform[] trms;//3°³
    [SerializeField] private float speed = 1.0f;

    float percent = 0.0f;

    [ContextMenu("restart")]
    [ExecuteInEditMode]
    public void Restart()
    {
        transform.localPosition = new Vector3(0, 0, 0);

        StartCoroutine(BazierCo());
    }

    private IEnumerator BazierCo()
    {
        float dt;
        float percent = 0.0f;
        Vector2 p1;
        Vector2 p2;
        Vector2 result;
        while(percent <= 1.0f)
        {
            dt = Time.deltaTime * speed;
            percent += dt;

            p1 = Vector2.Lerp(trms[0].position, trms[1].position, percent);
            p2 = Vector2.Lerp(trms[1].position, trms[2].position, percent);
            result = Vector2.Lerp(p1, p2, percent);

            transform.position = result;
            yield return null;
        }
    }
}
