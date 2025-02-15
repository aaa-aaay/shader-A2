using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class EffectManager : MonoBehaviour
{
    public Volume postProcessingVolume;

    private UnderwaterPostProcess underwaterEffect;
    private PixelationEffect pixelationEffect;
    private BlackWhitePostProcess blackAndWhiteEffect;

    public Material barrierMaterial;
    public Material barrierMaterial2;

    private Bloom bloom;
    private Tonemapping tonemapping;
    private Vignette vignette;

    private void Start()
    {
        // Initialize custom post-processing effects
        if (postProcessingVolume.profile.TryGet(out underwaterEffect))
        {
            underwaterEffect.isEnabled.value = false;
        }
        if (postProcessingVolume.profile.TryGet(out pixelationEffect))
        {
            pixelationEffect.isEnabled.value = false;
        }
        if (postProcessingVolume.profile.TryGet(out blackAndWhiteEffect))
        {
            blackAndWhiteEffect.isEnabled.value = false;
        }

        // Initialize built-in URP effects
        if (postProcessingVolume.profile.TryGet(out bloom))
        {
            bloom.active = false;
        }
        if (postProcessingVolume.profile.TryGet(out tonemapping))
        {
            tonemapping.active = false;
        }
        if (postProcessingVolume.profile.TryGet(out vignette))
        {
            vignette.active = false;
        }

        barrierMaterial.SetFloat("_Dissolve_Value", 0);
        barrierMaterial2.SetFloat("_Dissolve_Value", 0);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            pixelationEffect.isEnabled.value = !pixelationEffect.isEnabled.value;
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            blackAndWhiteEffect.isEnabled.value = !blackAndWhiteEffect.isEnabled.value;
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            underwaterEffect.isEnabled.value = !underwaterEffect.isEnabled.value;
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            bloom.active = !bloom.active;
        }

        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            tonemapping.active = !tonemapping.active;
        }

        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            vignette.active = !vignette.active;
        }

        if (Input.GetKeyDown(KeyCode.R)){
            SceneManager.LoadScene(0);
        }
    }
}
