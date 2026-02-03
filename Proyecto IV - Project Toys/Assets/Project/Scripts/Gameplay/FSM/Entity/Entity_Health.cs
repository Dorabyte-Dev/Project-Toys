using UnityEngine;

public class Entity_Health : MonoBehaviour
{
    public Entity entity;
    private Entity_Stats stats;
    [SerializeField] private Entity_VFX vfx;

    public float currentHp;
    public float maxHp;
    protected bool isDead;
    public bool invincibleMode; //If true, entity takes no damage

    public virtual void Awake()
    {
        entity = GetComponent<Entity>();
        stats = GetComponent<Entity_Stats>();
        vfx = GetComponent<Entity_VFX>();
        ResetStats();
        
    }

    public virtual void ResetStats()
    {
        currentHp = stats.GetMaxetHealth();
        maxHp = stats.GetMaxetHealth();
    }
    public virtual void TakeDamage(float takeDamage, Transform damageDealer)
    {
        if (isDead)
            return;
        vfx.DamageVFX_Feedback(damageDealer);
        ReduceHp(takeDamage);
    }

    public virtual void ReduceHp(float damage)
    {
        if(invincibleMode) return; //No damage taken in invincible mode
        currentHp -= damage;
        if (currentHp < 0)
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