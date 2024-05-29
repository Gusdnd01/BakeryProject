using AmplifyShaderEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessingManager : MonoSingleton<PostProcessingManager>
{
    [SerializeField] private Volume _volume;

    private VolumeProfile _volumeProfile = null;

    #region VolumeComponent
    private Bloom _bloom = null;
    private ChromaticAberration _chromatic = null;
    #endregion

    private void Awake()
    {
        _volumeProfile = _volume.profile;

        _volumeProfile.TryGet<Bloom>(out _bloom);
        _volumeProfile.TryGet<ChromaticAberration>(out _chromatic);
    }

    public void ModifyBloom(float value)
    {
        if(_bloom != null)
        {
            _bloom.intensity.value = value;
        }
    }

    public void ModifyChromatic(float value)
    {
        if (_chromatic !=  null)
        {
            _chromatic.intensity.value = value;
        }
    }
}
