using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;
using UnityEngine.InputSystem;

using Random = UnityEngine.Random;

using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class FeedbackManager : MonoSingleton<FeedbackManager>
{
    [SerializeField] private CinemachineImpulseSource _impulseSource;
    [SerializeField] private CinemachineVirtualCamera _cinemachineCam;
    [SerializeField] private VolumeProfile _volumeProfile;
    [SerializeField] private float _endSpeed = 1.0f;
    public float EndSpeed
    {
        get => _endSpeed;
        set => _endSpeed = value;
    }
    private CinemachineBasicMultiChannelPerlin _multiChannelPerlin;

    private bool _shakingInDuration = false;

    // 시간 관련
    private float _limitTime;
    private float _currentTime;

    private Bloom _bloom;

    private void Start()
    {
        _multiChannelPerlin = _cinemachineCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        if(_multiChannelPerlin == null)
        {
            _multiChannelPerlin.m_AmplitudeGain = 0.0f;
            _multiChannelPerlin.m_FrequencyGain = 0.0f;
        }

        GameObject volumeObj = new GameObject();
        volumeObj.name = "Global Volume";
        Volume volume = volumeObj.AddComponent<Volume>();
        volume.profile = _volumeProfile;

        if(volume.profile.TryGet<Bloom>(out _bloom))
        {
            _bloom.intensity.Override(1.0f);
        }


        DontDestroyOnLoad(volumeObj);
    }

    public void ShakeScreen(Vector3 dir)
    {
        _impulseSource.m_DefaultVelocity = dir;
        _impulseSource.GenerateImpulse();
    }

    public void ShakeScreen(float shakeValue)
    {
        /*Vector3 randomVector = new Vector3(Random.Range(-shakeValue, shakeValue), Random.Range(-shakeValue, shakeValue), Random.Range(-shakeValue, shakeValue));
        //_impulseSource.m_DefaultVelocity = Vector3.one * MaestrOffice.GetPlusOrMinus() * shakeValue;
        _impulseSource.m_DefaultVelocity = randomVector;
        _impulseSource.GenerateImpulse();*/

        _multiChannelPerlin.m_FrequencyGain = shakeValue;
        _multiChannelPerlin.m_AmplitudeGain = shakeValue;

        _shakingInDuration = true;
    }

    public void FreezeTime(float freezeValue, float freezeTime)
    {
        StartCoroutine(FreezeCo(freezeTime));
        Time.timeScale = freezeValue;
    }

    private IEnumerator FreezeCo(float time)
    {
        yield return new WaitForSeconds(time);
        Time.timeScale = 1;
    }

    private void FixedUpdate()
    {
        if (_multiChannelPerlin != null && !(_multiChannelPerlin.m_AmplitudeGain <= 0.0f || _multiChannelPerlin.m_FrequencyGain <= 0.0f))
        {
            if (_multiChannelPerlin.m_AmplitudeGain >= 0.1f)
            {
                _multiChannelPerlin.m_AmplitudeGain -= Time.deltaTime * _endSpeed;;
            }
            else
            {
                _multiChannelPerlin.m_AmplitudeGain = 0.0f;
            }

            if (_multiChannelPerlin.m_FrequencyGain >= 0.1f)
            {
                _multiChannelPerlin.m_FrequencyGain -= Time.deltaTime * _endSpeed;;
            }
            else
            {
                _multiChannelPerlin.m_FrequencyGain = 0.0f;
            }
        }

/*        if(_multiChannelPerlin.m_AmplitudeGain <= 0.0f || _multiChannelPerlin.m_FrequencyGain <= 0.0f)
        {
            _shakingInDuration = false;
        }
*/

        /*if (_shakingInDuration)
        {
            _currentTime += Time.fixedDeltaTime;
            _impulseSource.GenerateImpulse();
            if(_currentTime >= _limitTime)
            {
                _shakingInDuration = false;
            }
        }*/
    }
}
