using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class Enemy_VFX : Entity_VFX
{
    [SerializeField] private VisualEffect pDodgeShine;
    [SerializeField] private ParticleSystem pHitEffect;
    
    public void ShineEffect()
    {
        pDodgeShine.Play();
    }
    
    public void HitPSEffect(Quaternion particleRotation)
    {
        pHitEffect.transform.rotation = particleRotation;
        pHitEffect.Play();
    }
}