using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

public class CustomGrabFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class Settings
    {
        [Header("Pass Settings")]
        // 쉐이더에서 받아오는 GrabPass Texture2D 변수명
        public string Texture2DPropertyName = "_CustomCameraTexture";
        // 그랩패스 실행할 렌더 순서
        public RenderPassEvent passEvent = RenderPassEvent.AfterRenderingTransparents;
        // 그랩패스 다운 샘플링
        public Downsampling downsamplingMethod = Downsampling.None;

        // 캡처 오브젝트 커스텀 렌더 필터링 설정 (중복 GrabPass 방지)
        [Header("Target Object Settings")]
        public LayerMask layerMask;
        public string[] shaderTagStrings = new string[]
            {"UniversalForwardOnly", "SRPDefaultUnlit", "LightweightForward", "UniversalForward"};
    }

    // 피쳐 세팅
    [SerializeField] private Settings _settings;
    // 그랩 패스
    private GrabPass _grabPass;
    // 특정 오브젝트 커스텀 렌더 패스
    private FilterPass _renderPass;

    public override void Create()
    {

        List<ShaderTagId> shaderTagIds = new List<ShaderTagId>();
        for (int i = 0; i < _settings.shaderTagStrings.Length; i++)
        {
            shaderTagIds.Add(new ShaderTagId(_settings.shaderTagStrings[i]));
        }
        _grabPass = new GrabPass(shaderTagIds,_settings.passEvent, _settings.Texture2DPropertyName, _settings.downsamplingMethod, _settings.layerMask);
        _renderPass = new FilterPass(_settings.passEvent + 1, shaderTagIds, _settings.layerMask);
    }



    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        //_grabPass.Setup(renderer.cameraColorTargetHandle);

        renderer.EnqueuePass(_grabPass);
        renderer.EnqueuePass(_renderPass);
    }
}
