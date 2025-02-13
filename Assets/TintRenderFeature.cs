using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class TintRenderFeature : ScriptableRendererFeature
{
    private TintPass tintpass;

    public override void Create()
    {
        tintpass = new TintPass();
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(tintpass);
    }

    class TintPass : ScriptableRenderPass
    {
        private Material material;
        int tintID = Shader.PropertyToID("_temp");
        RenderTargetIdentifier src, tint;

        public TintPass()
        {
            if (!material)
            {
                material = CoreUtils.CreateEngineMaterial("Unlit/Post Process Test");
            }
            renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            RenderTextureDescriptor desc = renderingData.cameraData.cameraTargetDescriptor;
            src = renderingData.cameraData.renderer.cameraColorTarget;
            cmd.GetTemporaryRT(tintID, desc, FilterMode.Bilinear);
            tint = new RenderTargetIdentifier(tintID);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer commandBuffer = CommandBufferPool.Get("TintRenderFeature");
            VolumeStack volumes = VolumeManager.instance.stack;

            CustomPostScreenTint tintData = volumes.GetComponent<CustomPostScreenTint>();

            if (tintData != null && tintData.IsActive())
            {
                material.SetColor("_OverlayColor", tintData.tintColor.value);
                material.SetFloat("_Intensity", tintData.tintIntensity.value);

                Blit(commandBuffer, src, tint, material, 0);
                Blit(commandBuffer, tint, src);
            }

            context.ExecuteCommandBuffer(commandBuffer);
            CommandBufferPool.Release(commandBuffer);
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            cmd.ReleaseTemporaryRT(tintID);
        }
    }
}
