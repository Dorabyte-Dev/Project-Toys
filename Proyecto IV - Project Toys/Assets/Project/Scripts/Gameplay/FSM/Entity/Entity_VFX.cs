using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.VFX;
using Random = UnityEngine.Random;

public class Entity_VFX : MonoBehaviour
{
    private static readonly int FlashColorProperty = Shader.PropertyToID("_EmissiveColor");
    
    [Header("--- Core Components ---")]
    
    [SerializeField] protected Renderer[] renderMesh;
    [SerializeField] protected VisualEffect vfxGraph;

    [Header("--- Damage Flash (Color Change) ---")]
    public Color feedbackColor = Color.white;
    public float feedbackDuration = 0.1f;
    public float glowIntensity = 5f;
    
    // Datos internos para restaurar materiales
    protected List<Material[]> originalMaterials = new List<Material[]>();
    protected List<Color[]> originalColors = new List<Color[]>();
    protected Coroutine changeCoroutine;
    protected Coroutine revertCoroutine;

    [Header("--- Knockback (Push Feedback) ---")]
    [FormerlySerializedAs("pushStrengh")] 
    public float pushStrength = .4f;
    [FormerlySerializedAs("pushDuration")] 
    [Range(0, 1)] public float pushWaitDuration = 0.2f;

    [Tooltip("Umbral de velocidad que detendrá al enemigo del empuje")]
    [Range(0.001f, 0.1f)]public float pushStopThreeshold = 0.05f;

    [Header("--- Dissolve Effect ---")]
    public float dissolveRate = 0.0125f;
    [Range(0.01f, 0.1f)] public float refreshRate = 0.025f;

    protected List<Material[]> meshMaterials = new List<Material[]>();

    [Header("--- Entity Shake ---")]
    [SerializeField] private float shakeDuration = 0.2f;
    [SerializeField] private float shakeStrength = 0.5f;
    [SerializeField] private float randomShake = 0.1f;
    
    [Header("--- Hit Stop ---")]
    [SerializeField]private float hitStopModifier = 0.1f;
    [SerializeField]private float hitStopDuration = 0.1f;

    protected virtual void Awake()
    {
        SaveOriginalMaterials();
        /*if (renderMesh[0] != null)
        {
            for (int i = 0; i < renderMesh.Length; i++)
            {
                if (renderMesh[i] == null) continue;
                
            }
            //meshMaterials = renderMesh.materials;
        }*/
        SetMeshMaterials();
        glowIntensity *= 100;
    }

    private void SetMeshMaterials()
    {
        if(renderMesh[0] != null)
        {
            for (int i = 0; i < renderMesh.Length; i++)
            {
                if (renderMesh[i] == null) continue;
                meshMaterials.Add(renderMesh[i].materials);
            }
        }
    }

    private void SaveOriginalMaterials()
    {
        Renderer[] renderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (Renderer render in renderers)
        {
            List<Color> colors = new List<Color>();
            originalMaterials.Add(render.materials);
            for (int i = 0; i < render.materials.Length; i++)
            {
                if (render.materials[i].HasProperty(FlashColorProperty))
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

    public virtual IEnumerator HitStop(Entity entity)
    {
        entity.anim.SetFloat(nameof(hitStopModifier), hitStopModifier);
        yield return new WaitForSecondsRealtime(hitStopDuration);
        entity.anim.SetFloat(nameof(hitStopModifier), 1);
    }
    public virtual void DamageFeedback(Transform damageDealer)
    {
        TriggerMaterialChange();
        Vector3 pushDirection = (transform.position - damageDealer.position).normalized;
        StartCoroutine(PushFeedback(pushDirection));
        //Shake(shakeDuration, shakeStrength);
    }
    
    public virtual void DeathVFX_Feedback()
    {
        // La base no hace nada. Cada entidad decide su efecto de muerte.
    }

    #region ColoredFeedback
    public void TriggerMaterialChange()
    {
        if (changeCoroutine != null)
        {
            StopCoroutine(changeCoroutine);
            Debug.Log("<color=green>Stopped previous material change coroutine.</color>");
        }


        if (revertCoroutine != null)
        {
            StopCoroutine(revertCoroutine);
            Debug.Log("<color=green>Stopped previous material revert coroutine.</color>");
        }
        changeCoroutine = StartCoroutine(ChangeMaterialsTemporarily());
    }


    private IEnumerator ChangeMaterialsTemporarily()
    {
        ChangeMaterialsToColor(feedbackColor);
        Debug.Log("<color=green>Materials changed to feedback color.</color>");
        //yield return new WaitForSeconds(0.2f);
        yield return null;
        revertCoroutine = StartCoroutine(RevertMaterialsSmoothly(feedbackDuration));
    }


    private void ChangeMaterialsToColor(Color color)
    {
        Renderer[] skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (Renderer render in skinnedMeshRenderers)
        {
            Material[] newMaterials = new Material[render.materials.Length];
            for (int i = 0; i < render.materials.Length; i++)
            {
                newMaterials[i] = new Material(render.materials[i]);
                if (newMaterials[i].HasProperty(FlashColorProperty))
                {
                    newMaterials[i].EnableKeyword("_EMISSION");
                    
                    newMaterials[i].SetColor(FlashColorProperty, color * glowIntensity);
                    //newMaterials[i].color = color;
                }
            }
            render.materials = newMaterials;
        }
    }


    private IEnumerator RevertMaterialsSmoothly(float duration)
    {
        Renderer[] renderers = GetComponentsInChildren<SkinnedMeshRenderer>();

        List<Color[]> currentColors = new List<Color[]>();
        foreach (Renderer renderer in renderers)
        {
            List<Color> colors = new List<Color>();
            for (int i = 0; i < renderer.materials.Length; i++)
            {
                if (renderer.materials[i].HasProperty(FlashColorProperty))
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
                    if (renderers[i].materials[j].HasProperty(FlashColorProperty))
                    {
                        //renderers[i].materials[j].color = Color.Lerp(feedbackColor, originalColors[i][j], t);
                        renderers[i].materials[j].SetColor(FlashColorProperty, Color.Lerp(feedbackColor, originalColors[i][j], t));
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

    protected virtual IEnumerator PushFeedback(Vector3 direction)
    {
        //Add player movement disable
        
        // Rigidbody
        

        //Add player movement enable
        
        //CharacterController (?)
        /*
            CharacterController cc = GetComponent<CharacterController>();
            float elapsed = 0f;
            while (elapsed < pushDuration)
            {
                cc.Move(direction * pushStrengh * Time.unscaledDeltaTime);
                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }
        */
        
        return null;
    }

    
    // private void Shake(float duration, float magnitude)
    // {
    //     StartCoroutine(ShakeCoroutine(duration, magnitude));
    // }
    //
    // IEnumerator ShakeCoroutine(float duration, float magnitude)
    // {
    //     Debug.Log("Ejecutando VFX Shake");
    //     
    //     // Cogemos posicion del Rigibody ya que al aplicar este componente, normalmente que este efecto no
    //     // intervenga en el efecto de PushFeedback se realiza con rb.position y su funcion MovePosition
    //     Vector3 originalPosition = rb.position;
    //     float elapsed = 0f;
    //
    //     while (elapsed < duration)
    //     {
    //         float x = Random.Range(-randomShake, randomShake) * magnitude;
    //         float y = Random.Range(-randomShake, randomShake) * magnitude;
    //         float z = Random.Range(-randomShake, randomShake) * magnitude;
    //         
    //         // transform.position = originalPosition + new Vector3(x, y, z);
    //         rb.MovePosition(originalPosition + new Vector3(x, y, z));
    //         
    //         elapsed += Time.fixedDeltaTime;
    //         yield return new WaitForFixedUpdate();
    //     }
    //     
    //     // transform.position = originalPosition;
    //     rb.MovePosition(originalPosition);
    // }
}
