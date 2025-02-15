using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class NewBehaviourScript : MonoBehaviour
{
    public Volume postProcessingVolume;
    private UnderwaterPostProcess underwaterEffect;


    void Start()
    {
        if (postProcessingVolume.profile.TryGet(out underwaterEffect))
        {
            underwaterEffect.isEnabled.value = false;
        }
    }

    public void ToggleEffect()
    {
        if (underwaterEffect != null)
        {
            underwaterEffect.isEnabled.value = !underwaterEffect.isEnabled.value;
        }
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            ToggleEffect();
            return;
        }
    }

}
