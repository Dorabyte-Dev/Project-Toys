using UnityEngine;
using UnityEngine.VFX;

public class Enemy_VFX : Entity_VFX
{
    [SerializeField] private VisualEffect pDodgeShine;
    
    public void ShineEffect()
    {
        pDodgeShine.Play();
    }
}