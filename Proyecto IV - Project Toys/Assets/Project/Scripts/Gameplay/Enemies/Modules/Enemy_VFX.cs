using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
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
    [FormerlySerializedAs("pHitEffect")] [SerializeField] private GameObject pHitEffectParticles;
    [SerializeField] private GameObject pHitEffectFlash;
    [SerializeField] private Enemy _enemy;

    [Header("--- Enemy Extras ---")] 
    [SerializeField] private ParticleSystem enemyRangedChargeProjectile;
    [SerializeField] private ParticleSystem enemyMeleeChargeEffect;
    [SerializeField] private ParticleSystem enemyHitExecutionEffect;
    
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

    public override void DamageFeedback(Transform damageDealer)
    {
        base.DamageFeedback(damageDealer);
        Vector3 damageDirection = damageDealer.position - transform.position;
        HitPSEffect(Quaternion.LookRotation(damageDirection));
    }

    public void HitPSEffect(Quaternion particleRotation)
    {
        GameObject pHitEffectInstance = Instantiate(pHitEffectParticles, transform.position, Quaternion.identity);
        
        Vector3 directionToCamera = (Camera.main.transform.position - transform.position).normalized;
        
        GameObject pHitEffectFlashInstance = Instantiate(pHitEffectFlash, transform.position + directionToCamera * 1f, Quaternion.identity);
        pHitEffectInstance.transform.rotation = particleRotation;
        
        
        
        ParticleSystem pHitEffectPS = pHitEffectInstance.GetComponent<ParticleSystem>();
        ParticleSystem pHitEffectFlashPS = pHitEffectFlashInstance.GetComponent<ParticleSystem>();
        if (pHitEffectPS == null)
        {
            Debug.LogError("Hit Effect prefab does not have a ParticleSystem component.");
            return;
        }
        if (pHitEffectFlashPS == null)
        {
            Debug.LogError("Hit Effect Flash prefab does not have a ParticleSystem component.");
            return;
        }
        pHitEffectFlashPS.Play();
        pHitEffectPS.Play();
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
            if (!_enemy.agent.isOnNavMesh)
            {
                _enemy.agent.ResetPath();
            }
            _enemy.agent.enabled = false;
            rb.isKinematic = false;
            rb.AddForce(direction * pushStrength, ForceMode.VelocityChange);
            
            yield return new WaitUntil(() => rb.linearVelocity.magnitude <= pushStopThreeshold);
            //Debug.Log("PushFeedback stopped for " + this.gameObject.name);
            yield return new WaitForSeconds(pushWaitDuration);
            
            Debug.Log("PushFeedback finished for " + this.gameObject.name);
            ResetPushFeedback();
            
        }
        else
        {
            Debug.LogError("Rigidbody is null in PushFeedback");
        }
    }
    
    public void ResetPushFeedback()
    {
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.linearVelocity = Vector3.zero;
            _enemy.agent.Warp(transform.position);
            _enemy.agent.enabled = true;
        }
        else
        {
            Debug.LogError("Rigidbody is null in ResetPushFeedback");
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

        if (meshRenderer != null && meshMaterials.Count > 0)
        {
            float currentDissolve = meshMaterials[0][0].GetFloat("_DissolveAmount");
            while (currentDissolve < 1f)
            {
                currentDissolve += dissolveRate;
                for (int i = 0; i < meshMaterials.Count; i++)
                {
                    for(int j = 0; j < meshMaterials[i].Length; j++)
                    {
                        //currentDissolve = meshMaterials[i][j].GetFloat("_DissolveAmount");
                        meshMaterials[i][j].SetFloat("_DissolveAmount", currentDissolve);
                    }
                }
                yield return new WaitForSeconds(refreshRate);
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

    #region Enemy Extras

    public void PlayRangedChargeVFX(bool play)
    {
        if(enemyRangedChargeProjectile == null)
        {
            Debug.LogError("Enemy Ranged Charge Projectile is not assigned in: " + this.gameObject.name);
            return;
        }
        if (play)
        {
            enemyRangedChargeProjectile.Play();
        }
        else
        {
            enemyRangedChargeProjectile.Stop();
        }
    }
    
    public void PlayMeleeChargeVFX(bool play)
    {
        if(enemyMeleeChargeEffect == null)
        {
            Debug.LogError("Enemy Melee Charge Effect is not assigned in: " + this.gameObject.name);
            return;
        }
        if (play)
        {
            enemyMeleeChargeEffect.Play();
        }
        else
        {
            enemyMeleeChargeEffect.Stop();
        }
    }
    
    public void PlayHitExecutionVFX()
    {
        if(enemyHitExecutionEffect == null)
        {
            Debug.LogError("Enemy Hit Execution Effect is not assigned in: " + this.gameObject.name);
            return;
        }
        enemyHitExecutionEffect.Play();
    }

    #endregion
   
}