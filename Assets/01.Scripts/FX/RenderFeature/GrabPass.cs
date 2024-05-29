using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GrabPass : ScriptableRenderPass
{
    private const string PASS_NAME = "CustomGrabPass";

    private List<ShaderTagId> _shaderTagIds;

    private FilteringSettings _filteringSettings;
    private RenderStateBlock _renderStateBlock;

    private RenderTargetIdentifier _source;
    private RTHandle _destination;

    private Downsampling _downsamplingMethod;
    private string _globalProperty;

    private LayerMask _layerMask;

    public GrabPass(List<ShaderTagId> shaderTagIds, RenderPassEvent passEvent, string globalProperty, Downsampling downsampling, LayerMask layermask)
    {
        _shaderTagIds = shaderTagIds;

        renderPassEvent = passEvent;
        _globalProperty = globalProperty;
        _downsamplingMethod = downsampling;
        _layerMask = layermask;

        _filteringSettings = new FilteringSettings(RenderQueueRange.all, layermask);
        _renderStateBlock = new RenderStateBlock(RenderStateMask.Nothing);

        _destination = RTHandles.Alloc(_globalProperty, name:_globalProperty);
    }

    public void Setup(RenderTargetIdentifier source)
    {
        ConfigureInput(ScriptableRenderPassInput.Color);

        _source = source;
    }

    public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
    {
        RenderTextureDescriptor descriptor = cameraTextureDescriptor;

        // DownSampling 처리
        descriptor.msaaSamples = 2;
        descriptor.depthBufferBits = 0;
        if (_downsamplingMethod == Downsampling._2xBilinear)
        {
            descriptor.width /= 2;
            descriptor.height /= 2;
        }
        else if (_downsamplingMethod == Downsampling._4xBox || _downsamplingMethod == Downsampling._4xBilinear)
        {
           descriptor.width /= 4;
           descriptor.height /= 4;
        }

        // 임시 RT 생성
        // RendererData에서 Intermediate Texture옵션을 Always로 해야함.
        cmd.GetTemporaryRT(Shader.PropertyToID(_destination.name), descriptor, _downsamplingMethod == Downsampling.None ? FilterMode.Point : FilterMode.Bilinear);
        //cmd.GetTemporaryRT(Shader.PropertyToID(_destination.name), descriptor);
        // Global Shader Propertie 등록
        cmd.SetGlobalTexture(_globalProperty, Shader.PropertyToID(_destination.name));
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        CommandBuffer cmd = CommandBufferPool.Get(PASS_NAME);
        // context.ExecuteCommandBuffer(cmd);
        cmd.Clear();
        
        cmd.Blit(_source, Shader.PropertyToID(_destination.name));
        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }

    public override void FrameCleanup(CommandBuffer cmd)
    {
        cmd.ReleaseTemporaryRT(Shader.PropertyToID(_destination.name));
    }
}
