using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class NewBehaviourScript : MonoBehaviour
{
    public Volume postProcessingVolume; // Assign this in Inspector
    private UnderwaterPostProcess underwaterEffect;


    void Start()
    {
        if (postProcessingVolume.profile.TryGet(out underwaterEffect))
        {
            underwaterEffect.active = false;
        }
    }

    public void ToggleEffect()
    {
        if (underwaterEffect != null)
        {
            underwaterEffect.active = !underwaterEffect.active;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Triggered by: " + other.gameObject.name + " with tag: " + other.gameObject.tag);

        if (other.CompareTag("Player")) // Ensure correct case
        {
            Debug.Log("Water effect triggered");    
            ToggleEffect();
            return;
        }
    }

}
