using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class ExplosionScreen : MonoBehaviour
{
    //디버그용 스크립트입니다. 기능은 완성되어 있으니 가져다 쓰셔도 무방합니다.
    private Rigidbody[] _explosionRbs;

    [Header("Objs")]
    [SerializeField] private GameObject _renderCam;
    [SerializeField] private GameObject _shatterEffectObj;
    [SerializeField] private Transform _explosionTrm;
    [SerializeField] private ParticleSystem _swordScreenEffect;

    [Header("explosion status")]
    [SerializeField] private float _explosionPow;
    [SerializeField] private float _explosionRad;

    [Header("Timing")]
    [SerializeField] private float _delayTime;
    [SerializeField] private float _destroyTime;

    private void Update()
    {
        //디버그 인풋입니다.
        if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            StartCoroutine(Shatter());
        }
    }

    private IEnumerator Shatter()
    {
        //랜더 텍스쳐에 검기 나오기 전 한 프레임만 찍는 용도의 코드입니다.
        _renderCam.SetActive(true);
        yield return new WaitForEndOfFrame();
        _renderCam.SetActive(false);

        //이펙트 실행
        _swordScreenEffect.Play();
        yield return new WaitForSeconds(_delayTime);

        //shatter Effect Object 생성
        var obj = Instantiate(_shatterEffectObj, _renderCam.transform.position, Quaternion.identity);
        obj.transform.position += new Vector3(0,0,5);
        obj.transform.rotation = Quaternion.Euler(0, 0, 180);

        //오브젝트 자식들에 있는 Rigidbody들을 가져와서 ExplosionForce해줍니다.
        _explosionRbs = obj.GetComponentsInChildren<Rigidbody>();
        foreach(var rb in _explosionRbs)
        {
            rb.AddExplosionForce(_explosionPow, _explosionTrm.position, _explosionRad);
        }

        yield return new WaitForSeconds(_destroyTime);

        Destroy(obj);
    }
}
