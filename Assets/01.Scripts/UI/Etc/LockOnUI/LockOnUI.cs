using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LockOnUI : MonoBehaviour
{
    [SerializeField] private RectTransform _lockOnUI;
    private Sequence seq;

    private Entity target;

    public void HandleChangePlayerTarget(Entity e)
    {
        if (e is not null)
        {
            _lockOnUI.position = e.transform.position;
            _lockOnUI.rotation = Quaternion.Euler(0, 0, 45);
            _lockOnUI.localScale = Vector3.one;

            if (seq != null && !seq.IsComplete())
                seq.Kill();
            seq = DOTween.Sequence();
            seq.Append(_lockOnUI.DORotate(new Vector3(0, 0, 405), 2));
            seq.Join(_lockOnUI.DOScale(Vector3.one * 2, 2)).SetLoops(-1, LoopType.Yoyo);

            target = e;
            _lockOnUI.gameObject.SetActive(true); 
        }
        else
        {
            if (seq != null)
                seq.Kill();
            target = e;
            _lockOnUI.gameObject.SetActive(false);
        }

    }
    private void Update()
    {
        if (target != null)
            _lockOnUI.transform.position = target.transform.position;
    }


}
