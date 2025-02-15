using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[Serializable, VolumeComponentMenuForRenderPipeline("Custom/Pixelation Effect", typeof(UniversalRenderPipeline))]
public class PixelationEffect : VolumeComponent, IPostProcessComponent
{
    public BoolParameter isEnabled = new BoolParameter(false, true);
    public ClampedFloatParameter pixelSize = new ClampedFloatParameter(10f, 1f, 100f);
    public ClampedFloatParameter glitchStrength = new ClampedFloatParameter(0.1f, 0f, 1f);
    public ClampedFloatParameter scanlineIntensity = new ClampedFloatParameter(0.1f, 0f, 1f);
    public ClampedFloatParameter staticNoiseStrength = new ClampedFloatParameter(0.1f, 0f, 1f);

    public bool IsActive() => isEnabled.value && (pixelSize.value > 1f || glitchStrength.value > 0f);
    public bool IsTileCompatible() => false;
}