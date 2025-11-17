using UnityEngine;

public class Enemy_Health : Entity_Health
{
    [SerializeField] private Enemy enemy;

    public override void Awake()
    {
        base.Awake();
        enemy = GetComponent<Enemy>();
    }
    public override void TakeDamage(float takeDamage, Transform damageDealer)
    {
        base.TakeDamage(takeDamage, damageDealer);
        //enemy.ChangeFlintState();
        if (isDead)
            return;
    }

    public override void ReduceHp(float damage)
    {
        base.ReduceHp(damage);
        if (currentHp > 0)
        {
            FlintState();
        }
    }

    public void FlintState()
    {
        entity.ChangeFlintState();
    }
}
