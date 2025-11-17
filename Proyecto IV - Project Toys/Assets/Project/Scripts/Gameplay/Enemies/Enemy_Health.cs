using UnityEngine;

public class Enemy_Health : Entity_Health
{
    [SerializeField] private Enemy enemy;
    public EnemyUI enemyUI;
    public override void Awake()
    {
        base.Awake();
        enemy = GetComponent<Enemy>();
        if (this.GetComponent<EnemyUI>())
        {
            enemyUI = GetComponent<EnemyUI>();
        }
        else
        {
            Debug.LogError("EnemyUI and Enemy Health are not in the same object");
        }
    }
    public override void TakeDamage(float takeDamage, Transform damageDealer)
    {
        base.TakeDamage(takeDamage, damageDealer);
        //enemy.ChangeFlintState();
        enemyUI.RecieveDamage((int)takeDamage);
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

    private void FlintState()
    {
        entity.ChangeFlintState();
    }
}
