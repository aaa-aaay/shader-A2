using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[Serializable, VolumeComponentMenuForRenderPipeline("Custom/Underwater Post-Processing", typeof(UniversalRenderPipeline))]
public class UnderwaterPostProcess : VolumeComponent, IPostProcessComponent
{
    public BoolParameter isEnabled = new BoolParameter(false, true);

    public ClampedFloatParameter distortionStrength = new ClampedFloatParameter(0.1f, 0f, 1f);
    public ClampedFloatParameter fogIntensity = new ClampedFloatParameter(0.5f, 0f, 1f);
    public ColorParameter waterColor = new ColorParameter(new Color(0.0f, 0.4f, 0.6f));

    public bool IsActive()
    {
        // Effect is active only if isEnabled is true
        return (distortionStrength.value > 0f || fogIntensity.value > 0f);
    }

    public bool IsTileCompatible() => true;
}
    