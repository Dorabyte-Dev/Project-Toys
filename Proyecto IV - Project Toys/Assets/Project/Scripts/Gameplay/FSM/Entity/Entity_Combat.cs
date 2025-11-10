using UnityEngine;

public class Entity_Combat : MonoBehaviour
{
    [Header("Target Detection")]
    [SerializeField] private Transform targetCheck;
    [SerializeField] private float targetCheckRadius = 1;
    [SerializeField] private LayerMask whatIsTarget;

    [SerializeField] private Entity_Stats stats;
    [SerializeField] private float damage;

    private void Awake()
    {
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
        return Physics.OverlapSphere(targetCheck.position, targetCheckRadius, whatIsTarget);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(targetCheck.position, targetCheckRadius);
    }
}
