using UnityEngine;

public class Boss_Health : Entity_Health
{
    private Boss boss;
    public override void Awake()
    {
        base.Awake();
        boss = GetComponent<Boss>();
    }

    public override void TakeDamage(float damage, Transform damageDealer)
    {
        if(!boss.canBeDamaged) return;
        base.TakeDamage(damage, damageDealer);
    }

    public override void ReduceHp(float damage)
    {
        base.ReduceHp(damage);
        if (currentHp <= boss.bossCanBeExecutedHpThreshold)
        {
            boss.canBeExecuted = true;
        }
    }
}
