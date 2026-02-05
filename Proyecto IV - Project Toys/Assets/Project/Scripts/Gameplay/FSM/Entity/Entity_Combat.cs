using System;
using UnityEngine;
using UnityEngine.Events;

public class Entity_Combat : MonoBehaviour
{
    //POR FAVOR NECESITAMOS QUE HAYA UN PLAYER_COMBAT Y UN ENEMY_COMBAT
    
    [Header("Target Detection")]
    [SerializeField] public LayerMask whatIsTarget;

    [SerializeField] private Entity_Stats stats;
    [SerializeField] private Entity entity;
    [SerializeField] private float damage;
    [SerializeField] private float heavyDamage;
    public UnityEvent targetHit; //Used by Player in ComboBar and by Enemy in PerfectDodge
    private void Awake()
    {
        entity = GetComponent<Entity>();
        stats = GetComponent<Entity_Stats>();
        damage = stats.GetMaxAttack();
        heavyDamage = stats.GetMaxHeavyAttack();
    }
    public void PerformAttack()
    {
        //Debug.Log("Start Attack");
        foreach (var target in GetDetectedColliders())
        {
            Debug.Log(target.name);  
            Entity_Health targetHealth = target.GetComponent<Entity_Health>();
            //Debug.Log(target);
            if (targetHealth != null)
            {
                targetHealth?.TakeDamage(damage, this.transform);
                if(targetHealth.invincibleMode) return; //If target is invincible, do not trigger hit events
                targetHit?.Invoke();
            }
            else if(target.CompareTag("pDodge"))
            {
                //Debug.Log("Perfect Dodge Triggered");
                if(GetComponent<Enemy>() != null)
                {
                    PerfectDodgeManager.SetPerfectDodgeFlag(entity.gameObject);
                }
            }
            else if (target.CompareTag("dObject"))
            {
                Break_Object breakObject = target.GetComponent<Break_Object>();
                if (breakObject != null)
                    breakObject.ActivateDestruction();
            }
            else
            {
                Debug.LogWarning("Entity_Health not found on +"  + target.name);
            }
                
        }
        
    }
    public void PerformHeavyAttack()
    {
        foreach (var target in GetDetectedColliders())
        {
            Entity_Health targetHealth = target.GetComponent<Entity_Health>();
            Debug.Log(target);
            if (targetHealth != null)
                targetHealth?.TakeDamage(heavyDamage, this.transform);
            else
                Debug.LogWarning("Entity_Health not found on +"  + target.name);
        }
    }

    private Collider[] GetDetectedColliders()
    {
        return Physics.OverlapSphere(entity.targetCheck.position, entity.targetCheckRadius, whatIsTarget);
    }
}