using UnityEngine;

public class Entity_Combat : MonoBehaviour
{
    [Header("Target Detection")]
    [SerializeField] private LayerMask whatIsTarget;

    [SerializeField] private Entity_Stats stats;
    [SerializeField] private Entity entity;
    [SerializeField] private float damage;

    private void Awake()
    {
        entity = GetComponent<Entity>();
        stats = GetComponent<Entity_Stats>();
        damage = stats.GetMaxAttack();
    }
    public void PerformAttack()
    {
        foreach (var target in GetDetectedColliders())
        {
            Entity_Health targetHealth = target.GetComponent<Entity_Health>();
            if (targetHealth != null)
            targetHealth?.TakeDamage(damage, this.transform);
            
        }
    }

    private Collider[] GetDetectedColliders()
    {
        return Physics.OverlapSphere(entity.targetCheck.position, entity.targetCheckRadius, whatIsTarget);
    }

    
}
