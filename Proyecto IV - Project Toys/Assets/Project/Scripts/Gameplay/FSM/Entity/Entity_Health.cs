using UnityEngine;

public class Entity_Health : MonoBehaviour
{
    public Entity entity;
    [SerializeField] protected Entity_VFX vfx;

    public float currentHp;
    public float maxHp;
    public bool isDead;
    public bool invincibleMode; //If true, entity takes no damage

    public virtual void Awake()
    {
        entity = GetComponent<Entity>();
        vfx = GetComponent<Entity_VFX>();
        ResetStats();
        
    }

    public virtual void ResetStats()
    {
        currentHp = maxHp;
        isDead = false;
    }
    public virtual void TakeDamage(float takeDamage, Transform damageDealer)
    {
        if (isDead)
            return;
        vfx.DamageFeedback(damageDealer);
        ReduceHp(takeDamage);
    }

    public virtual void ReduceHp(float damage)
    {
        if(invincibleMode) return; //No damage taken in invincible mode
        currentHp -= damage;
        if (currentHp <= 0)
        {
            IsDead();
        }
    }


    public virtual void IsDead()
    {
        isDead = true;
        entity.DeadEntity();
        vfx.DeathVFX_Feedback();
    }
}