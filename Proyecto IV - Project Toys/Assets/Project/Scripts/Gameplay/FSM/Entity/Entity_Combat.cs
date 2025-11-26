using UnityEngine;

public class Entity_Combat : MonoBehaviour
{
    [Header("Target Detection")]
    [SerializeField] private LayerMask whatIsTarget;

    [SerializeField] private Entity_Stats stats;
    [SerializeField] private Entity entity;
    [SerializeField] private float damage;
    [SerializeField] private float heavyDamage;

    private void Awake()
    {
        entity = GetComponent<Entity>();
        stats = GetComponent<Entity_Stats>();
        damage = stats.GetMaxAttack();
        heavyDamage = stats.GetMaxHeavyAttack();
    }
    public void PerformAttack()
    {
        foreach (var target in GetDetectedColliders())
        {
            Entity_Health targetHealth = target.GetComponent<Entity_Health>();
            if (targetHealth != null)
                targetHealth?.TakeDamage(damage, this.transform);
            else
                Debug.LogWarning("Entity_Health not found on +"  + target.name);
        }
    }
    public void PerformHeavyAttack()
    {
        foreach (var target in GetDetectedColliders())
        {
            Entity_Health targetHealth = target.GetComponent<Entity_Health>();
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
