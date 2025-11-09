using UnityEngine;

public class Entity_Health : MonoBehaviour
{
    private Entity entity;
    private Entity_Stats stats;

    [SerializeField] protected float currentHp;
    [SerializeField] protected float maxHp;
    protected bool isDead;

    private void Awake()
    {
        entity = GetComponent<Entity>();
        stats = GetComponent<Entity_Stats>();

        currentHp = stats.GetMaxetHealth();
    }

    public virtual void TakeDamage(float takeDamage, Transform damageDealer)
    {
        if (isDead)
            return;
        ReduceHp(takeDamage);
    }

    protected void ReduceHp(float damage)
    {
        currentHp -= damage;
        if (maxHp < 0)
        {
            IsDead();
        }
    }


    public virtual void IsDead()
    {
        isDead = true;
    }
}
