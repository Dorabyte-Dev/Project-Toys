using UnityEngine;

public class Entity_Health : MonoBehaviour
{
    protected float maxHp;
    protected bool isDead;

    public void TakeDamage(float takeDamage)
    {
        if (isDead)
            return;
        ReduceHp(takeDamage);
    }

    protected void ReduceHp(float damage)
    {
        maxHp -= damage;
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
