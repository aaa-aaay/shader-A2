using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class UnderwaterRenderPassFeature : ScriptableRendererFeature
{
    private UnderwaterPass underwaterPass;

    public override void Create()
    {
        underwaterPass = new UnderwaterPass();
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(underwaterPass);
    }

    class UnderwaterPass : ScriptableRenderPass
    {
        Material _mat;
        int tempRT = Shader.PropertyToID("_UnderwaterEffectTemp");
        RenderTargetIdentifier source, underwaterRT;

        public UnderwaterPass()
        {
            if (!_mat)
            {

                _mat = CoreUtils.CreateEngineMaterial("Custom Post-Processing/UnderwaterEffect");

            }

            renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            RenderTextureDescriptor desc = renderingData.cameraData.cameraTargetDescriptor;
            source = renderingData.cameraData.renderer.cameraColorTargetHandle;

            cmd.GetTemporaryRT(tempRT, desc, FilterMode.Bilinear);

            underwaterRT = new RenderTargetIdentifier(tempRT);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get("UnderwaterRenderPassFeature");
            VolumeStack stack = VolumeManager.instance.stack;
            UnderwaterPostProcess effect = stack.GetComponent<UnderwaterPostProcess>();

            if (effect == null || !effect.IsActive()) return; 

            _mat.SetFloat("_DistortionStrength", effect.distortionStrength.value);
            _mat.SetFloat("_FogIntensity", effect.fogIntensity.value);
            _mat.SetColor("_WaterColor", effect.waterColor.value);
            //_mat.SetTexture("_DistortionTex", effect.noiseTexture.value);
            _mat.SetTexture("_CausticTex", effect.CausticTexture.value);
            _mat.SetFloat("_CausticStrength", effect.CausticStrength.value);

            _mat.SetFloat("_ChromaticAmount", effect.ChromaticAmount.value);

            Blit(cmd, source, underwaterRT, _mat, 0);
            Blit(cmd, underwaterRT, source);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }


        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            cmd.ReleaseTemporaryRT(tempRT);
        }
    }
}
