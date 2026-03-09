using System;
using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class Enemy_VFX : Entity_VFX
{
    [SerializeField] protected Rigidbody rb;
    [SerializeField] private VisualEffect pDodgeShine;
    [SerializeField] private ParticleSystem pHitEffect;
    
    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody>();
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

    public override void DeathVFX_Feedback()
    {
        base.DeathVFX_Feedback();
        StartCoroutine(Dissolve());
    }

    protected override IEnumerator PushFeedback(Vector3 direction)
    {
        if (rb != null)
        {
            rb.AddForce(direction * pushStrength, ForceMode.VelocityChange);
            yield return new WaitForSeconds(pushDuration);
            rb.linearVelocity = Vector3.zero;
        }
        else
        {
            Debug.LogError("Rigidbody is null in PushFeedback");
        }
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

    
   
}