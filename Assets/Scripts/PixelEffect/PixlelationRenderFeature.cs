using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;


public class PixelationRenderFeature : ScriptableRendererFeature
{
    class PixelationPass : ScriptableRenderPass
    {
        private Material matt;
        private RenderTargetIdentifier source;
        private RenderTargetIdentifier tempTarget;
        private int tempRT;

        public PixelationPass()
        {
            if (!matt)
                matt = CoreUtils.CreateEngineMaterial("Custom Post-Processing/PixelationEffect");

            renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            RenderTextureDescriptor desc = renderingData.cameraData.cameraTargetDescriptor;
            source = renderingData.cameraData.renderer.cameraColorTargetHandle;
            tempRT = Shader.PropertyToID("_PixelationTempRT");  
            cmd.GetTemporaryRT(tempRT, desc, FilterMode.Point);
            tempTarget = new RenderTargetIdentifier(tempRT);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get("PixelationEffect");
            VolumeStack stack = VolumeManager.instance.stack;
            PixelationEffect effect = stack.GetComponent<PixelationEffect>();

            if (effect == null || !effect.IsActive())
                return;

            matt.SetFloat("_PixelSize", effect.pixelSize.value);
            matt.SetFloat("_GlitchStrength", effect.glitchStrength.value);
            matt.SetFloat("_ScanlineIntensity", effect.scanlineIntensity.value);
            matt.SetFloat("_StaticNoiseStrength", effect.staticNoiseStrength.value);

            Blit(cmd, source, tempTarget, matt, 0);
            Blit(cmd, tempTarget, source);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            cmd.ReleaseTemporaryRT(tempRT);
        }
    }

    private PixelationPass pixelationPass;

    public override void Create()
    {
        pixelationPass = new PixelationPass();
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(pixelationPass);
    }
}
