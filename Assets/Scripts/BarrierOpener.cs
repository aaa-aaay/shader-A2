using System.Collections;
using UnityEngine;

public class BarrierOpener : MonoBehaviour
{
    public GameObject textCanvas;
    public Material barrierMaterial;
    public SphereCollider barriercollider;

    private float dissolveValue = 0f;
    private float dissolveDuration = 1.5f;
    private bool playerInTrigger = false; 

    void Update()
    {
        if (playerInTrigger && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Dissolve Started!");
            StartCoroutine(DissolveRoutine(0f, 1f, dissolveDuration));
            barriercollider.enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            textCanvas.gameObject.SetActive(true);
            playerInTrigger = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            textCanvas.gameObject.SetActive(false);
            playerInTrigger = false; 
        }
    }

    IEnumerator DissolveRoutine(float startValue, float endValue, float duration)
    {
        //DESTORY THE BARRIER


        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            dissolveValue = Mathf.Lerp(startValue, endValue, elapsedTime / duration);
            barrierMaterial.SetFloat("_Dissolve_Value", dissolveValue);
            elapsedTime += Time.deltaTime;
            yield return null; 
        }

        barrierMaterial.SetFloat("_Dissolve_Value", endValue);
    }
}
