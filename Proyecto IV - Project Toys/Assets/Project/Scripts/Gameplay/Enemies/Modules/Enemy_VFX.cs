using System;
using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class Enemy_VFX : Entity_VFX
{
    [Header("--- Glow Effect ---")]
    public Material glowMaterial;
    private Material[] originalMaterials;
    private Renderer meshRenderer;
    [Space(20)]
    [SerializeField] protected Rigidbody rb;
    [SerializeField] private VisualEffect pDodgeShine;
    [SerializeField] private ParticleSystem pHitEffect;
    [SerializeField] private Enemy _enemy;
    
    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody>();
        _enemy = GetComponent<Enemy>();
        
        meshRenderer = GetComponentInChildren<Renderer>();
        originalMaterials = meshRenderer.materials;
    }
    public event Action OnDissolveComplete;
    
    public void ShineEffect()
    {
        pDodgeShine.Play();
    }
    
    public void HitPSEffect(Quaternion particleRotation)
    {
        pHitEffect.transform.rotation = particleRotation;
        pHitEffect.Play();
    }
    
    public void HitStop()
    {
        StartCoroutine(base.HitStop(_enemy));
    }
    public override void DeathVFX_Feedback()
    {
        base.DeathVFX_Feedback();
        StartCoroutine(Dissolve());
    }

    protected override IEnumerator PushFeedback(Vector3 direction)
    {
        if (rb != null)
        {
            Debug.Log("PushFeedback started for " + this.gameObject.name);
            
            _enemy.agent.enabled = false;
            rb.isKinematic = false;
            rb.AddForce(direction * pushStrength, ForceMode.VelocityChange);
            
            yield return new WaitForSeconds(pushDuration);
            
            rb.isKinematic = true;
            rb.linearVelocity = Vector3.zero;
            _enemy.agent.enabled = true;
            
            Debug.Log("PushFeedback finished for " + this.gameObject.name);
        }
        else
        {
            Debug.LogError("Rigidbody is null in PushFeedback");
        }
    }
    
    public void StartPushFeedback(Vector3 direction)
    {
        StartCoroutine(PushFeedback(direction));
    }

    #region DissolveFeedback
    public IEnumerator Dissolve()
    {
        if (vfxGraph)
        {
            vfxGraph.Play();
        }

        if (meshMaterials != null)
        {
            if (meshMaterials.Length > 0)
            {
                while(meshMaterials[0].GetFloat("_DissolveAmount") < 1f)
                {
                    for (int i = 0; i < meshMaterials.Length; i++)
                    {
                        float currentDissolve = meshMaterials[i].GetFloat("_DissolveAmount");
                        meshMaterials[i].SetFloat("_DissolveAmount", currentDissolve + dissolveRate);
                    }
                    yield return new WaitForSeconds(refreshRate);
                }
            }
        }
        OnDissolveComplete?.Invoke();
    }
    

    #endregion

    public void GlowEffect()
    {
        if (meshRenderer != null && glowMaterial != null)
        {
            Material[] newMaterials = new Material[meshRenderer.materials.Length + 1];
            for (int i = 0; i < meshRenderer.materials.Length; i++)
            {
                newMaterials[i] = meshRenderer.materials[i];
            }
            newMaterials[meshRenderer.materials.Length] = glowMaterial;
            
            meshRenderer.materials = newMaterials;
        }
        else
        {
            Debug.LogError("Mesh Renderer or Glow Material is not assigned in: " + this.gameObject.name);
        }
    }
    
    public void RemoveGlowEffect()
    {
        if (meshRenderer != null && originalMaterials != null)
        {
            meshRenderer.materials = originalMaterials;
        }
        else
        {
            Debug.LogError("Mesh Renderer or Original Materials is not assigned in: " + this.gameObject.name);
        }
    }
   
}