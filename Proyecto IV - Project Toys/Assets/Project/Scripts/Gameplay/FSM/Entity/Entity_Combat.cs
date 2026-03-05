using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class Entity_Combat : MonoBehaviour
{
    [Header("Target Detection")]
    [SerializeField] public LayerMask whatIsTarget;

    [SerializeField] private Entity entity;
    [SerializeField] protected float baseDamage;
    public UnityEvent targetHit;
    public float finalDamage;

    public virtual void Awake()
    {
        entity = GetComponent<Entity>();
    }

    public virtual void PerformAttack()
    {
        foreach (var target in GetDetectedColliders())
        {
            Debug.Log(target.name);
            Entity_Health targetHealth = target.GetComponent<Entity_Health>();
            if (targetHealth != null)
            {
                targetHealth.TakeDamage(baseDamage, this.transform);
                if (targetHealth.invincibleMode) return;
                targetHit?.Invoke();
            }
            else if (target.CompareTag("pDodge"))
            {
                if (GetComponent<Enemy>() != null)
                    PerfectDodgeManager.SetPerfectDodgeFlag(entity.gameObject);
            }
            else if (target.CompareTag("dObject"))
            {
                Break_Object breakObject = target.GetComponent<Break_Object>();
                if (breakObject != null)
                    breakObject.ActivateDestruction();
            }
            else
            {
                Debug.LogWarning("Entity_Health not found on " + target.name);
            }
        }
    }

    // public virtual void PerformHeavyAttack()
    // {
    //     foreach (var target in GetDetectedColliders())
    //     {
    //         Entity_Health targetHealth = target.GetComponent<Entity_Health>();
    //         Debug.Log(target);
    //         if (targetHealth != null)
    //             targetHealth.TakeDamage(heavyDamage, this.transform);
    //         else
    //             Debug.LogWarning("Entity_Health not found on " + target.name);
    //     }
    // }

    protected Collider[] GetDetectedColliders()
    {
        return Physics.OverlapSphere(entity.targetCheck.position, entity.targetCheckRadius, whatIsTarget);
    }
}