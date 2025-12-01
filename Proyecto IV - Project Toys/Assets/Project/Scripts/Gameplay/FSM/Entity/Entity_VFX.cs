using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity_VFX : MonoBehaviour
{
    private static readonly int Color1 = Shader.PropertyToID("_Color");
    private List<Material[]> originalMaterials = new List<Material[]>();
    private List<Color[]> originalColors = new List<Color[]>();
    private Coroutine changeCoroutine;
    private Coroutine revertCoroutine;
    [SerializeField] private Rigidbody rb;
    public Color feedbackColor;
    public float feedbackDuration;
    public float pushStrengh;
    [Range(0, 1)] public float pushDuration;
    [SerializeField] private float shakeDuration = 1;
    [SerializeField] private float shakeStrength = 1;
    [SerializeField] private float randomShake = 0.2f;

    void Awake()
    {
        SaveOriginalMaterials();
        rb = GetComponent<Rigidbody>();
    }

    private void SaveOriginalMaterials()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer render in renderers)
        {
            List<Color> colors = new List<Color>();
            originalMaterials.Add(render.materials);
            for (int i = 0; i < render.materials.Length; i++)
            {
                if (render.materials[i].HasProperty(Color1))
                {
                    colors.Add(render.materials[i].color);
                }
                else
                {
                    colors.Add(Color.black);
                }
            }
            originalColors.Add(colors.ToArray());
        }
    }
    // private void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.P))
    //     {
    //         TriggerMaterialChange();
    //         StartCoroutine(PushFeedback());
    //     }
    // }

    public void DamageVFX_Feedback()
    {
        TriggerMaterialChange();
        StartCoroutine(PushFeedback());
        Shake(shakeDuration, shakeStrength);
    }

    #region ColoredFeedback
    public void TriggerMaterialChange()
    {
        if (changeCoroutine != null)
        {
            StopCoroutine(changeCoroutine);
        }


        if (revertCoroutine != null)
        {
            StopCoroutine(revertCoroutine);
        }
        changeCoroutine = StartCoroutine(ChangeMaterialsTemporarily());
    }


    private IEnumerator ChangeMaterialsTemporarily()
    {
        ChangeMaterialsToColor(feedbackColor);
        //yield return new WaitForSeconds(0.2f);
        yield return null;
        revertCoroutine = StartCoroutine(RevertMaterialsSmoothly(feedbackDuration));
    }


    private void ChangeMaterialsToColor(Color color)
    {
        Renderer[] skinnedMeshRenderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer render in skinnedMeshRenderers)
        {
            Material[] newMaterials = new Material[render.materials.Length];
            for (int i = 0; i < render.materials.Length; i++)
            {
                // Create a new temporary material instance
                newMaterials[i] = new Material(render.materials[i]);
                if (newMaterials[i].HasProperty("_Color"))
                {
                    newMaterials[i].color = color;
                }
            }
            render.materials = newMaterials;
        }
    }


    private IEnumerator RevertMaterialsSmoothly(float duration)
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();


        List<Color[]> currentColors = new List<Color[]>();
        foreach (Renderer renderer in renderers)
        {
            List<Color> colors = new List<Color>();
            for (int i = 0; i < renderer.materials.Length; i++)
            {
                if (renderer.materials[i].HasProperty("_Color"))
                {
                    colors.Add(renderer.materials[i].color);
                }
                else
                {
                    colors.Add(Color.black); // Dummy color for materials without _Color property
                }
            }
            currentColors.Add(colors.ToArray());
        }


        float elapsedTime = 0f;


        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;


            for (int i = 0; i < renderers.Length; i++)
            {
                for (int j = 0; j < renderers[i].materials.Length; j++)
                {
                    if (renderers[i].materials[j].HasProperty("_Color"))
                    {
                        renderers[i].materials[j].color = Color.Lerp(feedbackColor, originalColors[i][j], t);
                    }
                }
            }


            yield return null;
        }


        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].materials = originalMaterials[i];
        }
    }
    #endregion
    private IEnumerator PushFeedback()
    {
        rb.AddForce(-transform.forward * pushStrengh, ForceMode.VelocityChange);    //En el caso normal ser�a la direcci�n del ataque, pero aun no tengo como comprobarlo
        yield return new WaitForSecondsRealtime(pushDuration);
        rb.linearVelocity = Vector3.zero;
    }

    private void Shake(float duration, float magnitude)
    {
        StartCoroutine(ShakeCoroutine(duration, magnitude));
    }
    
    IEnumerator ShakeCoroutine(float duration, float magnitude)
    {
        Debug.Log("Ejecutando VFX Shake");
        
        // Cogemos posicion del Rigibody ya que al aplicar este componente, normalmente que este efecto no
        // intervenga en el efecto de PushFeedback se realiza con rb.position y su funcion MovePosition
        Vector3 originalPosition = rb.position;
        float elapsed = 0f;
    
        while (elapsed < duration)
        {
            float x = Random.Range(-randomShake, randomShake) * magnitude;
            float y = Random.Range(-randomShake, randomShake) * magnitude;
            float z = Random.Range(-randomShake, randomShake) * magnitude;
            
            // transform.position = originalPosition + new Vector3(x, y, z);
            rb.MovePosition(originalPosition + new Vector3(x, y, z));
            
            elapsed += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        
        // transform.position = originalPosition;
        rb.MovePosition(originalPosition);
    }
}
