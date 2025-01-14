using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class BWRenderPassFeature : ScriptableRendererFeature
{
    private BWPass bwPass;
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        // Enqueue the custom render pass
        renderer.EnqueuePass(bwPass);
    }
        
    public override void Create()
    {
       bwPass = new BWPass();
    }

    class BWPass : ScriptableRenderPass
    {
        Material _mat; // Material for the black and white effect
        int bwID = Shader.PropertyToID("_Temp"); // property ID for the temporary render target
        RenderTargetIdentifier src, bw; // Render target indentifiers

        public BWPass()
        {
            // Create the material for the black and white effect
            if (!_mat)
            {
                _mat = CoreUtils.CreateEngineMaterial("Custom Post-Processing/B&W Post-Processing");
            }

            // Set the render pass event to execute before post-processing
            renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;

        }

        // Called when setting up the camera for rendering
        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            // Get the camera target descriptor
            RenderTextureDescriptor desc = renderingData.cameraData.cameraTargetDescriptor;
            // Set the source render target identifier
            src = renderingData.cameraData.renderer.cameraColorTargetHandle;
            // Create a temporary render target and get its identifier
            cmd.GetTemporaryRT(bwID, desc, FilterMode.Bilinear);
            bw = new RenderTargetIdentifier(bwID);
        }

        // Execute the custom render pass
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer commandBuffer = CommandBufferPool.Get("BWRenderPassFeature");
            // Access the volume stack and get the BlackAndWhitePostProcess component
            VolumeStack volumes = VolumeManager.instance.stack;
            BlackWhitePostProcess bWPP = volumes.GetComponent<BlackWhitePostProcess>();

            // Check if the black and white post-processing is active
            if (bWPP.IsActive())
            {
                // Set the blend intensity in the material
                _mat.SetFloat("_blend", (float)bWPP.blendIntensity);
                // Apply the black and white effect to the temporary render target
                Blit(commandBuffer, src, bw, _mat, 0);
                // Blit the result back to the source render target
                Blit(commandBuffer, bw, src);
            }

            // Execute the command buffer
            context.ExecuteCommandBuffer(commandBuffer);
            // Release the command buffer
            CommandBufferPool.Release(commandBuffer);
        }


        // Called when cleaning up the camera after rendering
        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            // Release the temporary render target
            cmd.ReleaseTemporaryRT(bwID);
        }

    }
}
