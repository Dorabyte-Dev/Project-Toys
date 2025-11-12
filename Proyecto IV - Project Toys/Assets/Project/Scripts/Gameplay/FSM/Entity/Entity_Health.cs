using UnityEngine;

public class Entity_Health : MonoBehaviour
{
    private Entity entity;
    private Entity_Stats stats;
    [SerializeField] private Entity_VFX vfx;

    [SerializeField] public float currentHp;
    [SerializeField] public float maxHp;
    protected bool isDead;

    private void Awake()
    {
        entity = GetComponent<Entity>();
        stats = GetComponent<Entity_Stats>();
        vfx = GetComponent<Entity_VFX>();

        currentHp = stats.GetMaxetHealth();
        maxHp = stats.GetMaxetHealth();
    }

    public virtual void TakeDamage(float takeDamage, Transform damageDealer)
    {
        if (isDead)
            return;
        vfx.DamageVFX_Feedback();
        ReduceHp(takeDamage);
    }

    protected void ReduceHp(float damage)
    {
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
    }
}
