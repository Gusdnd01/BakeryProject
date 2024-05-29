using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CustomRenderPassFeature : ScriptableRendererFeature
{
    class CustomRenderPass : ScriptableRenderPass
    {
        public RenderTargetIdentifier source;
        private Material material;
        private RTHandle tmpRenderHandler;

        public CustomRenderPass(Material material)
        {
            this.material = material;
            tmpRenderHandler = RTHandles.Alloc("_TemporaryColorTexture");
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get();

            cmd.Clear();
            cmd.GetTemporaryRT(Shader.PropertyToID(tmpRenderHandler.name), renderingData.cameraData.cameraTargetDescriptor);
            cmd.Blit(source, tmpRenderHandler.nameID, material);
            cmd.Blit(tmpRenderHandler.nameID, source);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        // Cleanup any allocated resources that were created during the execution of this render pass.
        public override void OnCameraCleanup(CommandBuffer cmd)
        {
        }
    }

    [System.Serializable]
    public class Settings
    {
        public Material material;
        public RenderPassEvent renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
    }

    private CustomRenderPass m_ScriptablePass;
    public Settings settings = null;

    public override void Create()
    {
        m_ScriptablePass = new CustomRenderPass(settings.material);

        m_ScriptablePass.renderPassEvent = settings.renderPassEvent;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(m_ScriptablePass);
    }
}


