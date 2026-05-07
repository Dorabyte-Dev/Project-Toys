using UnityEngine;

public class Enemy_Combat : Entity_Combat
{
    public BoxCollider attackCollider;
    
    private Enemy enemy;
    
    public override void Awake()
    {
        base.Awake();
        enemy = GetComponent<Enemy>();
    }
    public override void PerformAttack()
    {
        //base.PerformAttack();
        Collider[] hitColliders = GetDetectedColliders();
        foreach (var target in hitColliders)
        {
            Entity_Health targetHealth = target.GetComponent<Entity_Health>();
            if (targetHealth != null)
            {
                if(targetHealth.isDead) continue;
                targetHit?.Invoke();
                targetHealth.TakeDamage(baseDamage, this.transform);
            }
            else
            {
                Debug.LogWarning("Entity_Health not found on " + target.name);
            }
        }
    }
    
    public float GetBaseDamage()
    {
        return baseDamage;
    }

    public void SetPerfectDodgeCollider(bool isActive)
    {
        attackCollider.gameObject.SetActive(isActive);
    }

    protected override Collider[] GetDetectedColliders()
    {
        BoxCollider colliderUsed = attackCollider;
        Debug.Log("Box used: " + colliderUsed.name);
        Vector3 centerPoint = colliderUsed.transform.TransformPoint(colliderUsed.center);
        
        Vector3 halfExtents = Vector3.Scale(colliderUsed.size, transform.lossyScale) * 0.5f;

        Quaternion rotation = transform.rotation;

        return Physics.OverlapBox(centerPoint, halfExtents, rotation, whatIsTarget);
    }
}