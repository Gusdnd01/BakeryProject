using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class FilterPass : ScriptableRenderPass
{
    private List<ShaderTagId> _shaderTagIdList;

    private FilteringSettings _filteringSettings;
    private RenderStateBlock _renderStateBlock;

    public FilterPass(RenderPassEvent passEvent, List<ShaderTagId> shaderTagIds, LayerMask layerMask)
    {
        renderPassEvent = passEvent;
        _shaderTagIdList = shaderTagIds;
        _filteringSettings = new FilteringSettings(RenderQueueRange.all, layerMask);
        _renderStateBlock = new RenderStateBlock(RenderStateMask.Nothing);

    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        ConfigureInput(ScriptableRenderPassInput.Color);
        CommandBuffer cmd = CommandBufferPool.Get();
        // context.ExecuteCommandBuffer(cmd);
        cmd.Clear();
        DrawingSettings drawSettings;
        drawSettings = CreateDrawingSettings(_shaderTagIdList, ref renderingData, SortingCriteria.CommonTransparent);
        context.DrawRenderers(renderingData.cullResults, ref drawSettings, ref _filteringSettings, ref _renderStateBlock);
        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }
}
