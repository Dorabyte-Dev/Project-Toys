using UnityEngine;

public class Enemy_Health : Entity_Health
{
    [SerializeField] private Enemy enemy;
    public EnemyUI enemyUI;

    [HideInInspector] public float damageReceived;
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

    public override void ResetStats()
    {
        base.ResetStats();
    }

    public override void TakeDamage(float takeDamage, Transform damageDealer)
    {
        base.TakeDamage(takeDamage, damageDealer);
        //enemy.ChangeFlintState();
        /*Vector3 damageDirection = damageDealer.position - transform.position;
        if(damageDirection != Vector3.zero)
            enemy._vfx.HitPSEffect(Quaternion.LookRotation(damageDirection));*/
        enemy._vfx.DamageFeedback(damageDealer);
        enemyUI.ReceiveDamage((int)takeDamage);
        if (isDead)
            return;
    }

    public override void ReduceHp(float damage)
    {
        base.ReduceHp(damage);
        enemy.ChangeFlinchState();
        damageReceived += damage;
        /*if (currentHp > 0)
        {
            FlintState();
        }*/
    }

    public void Executed()
    {
        if (isDead) return;
        /*currentHp = 0;
        IsDead();*/
        DeadByExecution();
    }

    /*public override void IsDead()
    {
        isDead = true;
        vfx.DeathVFX_Feedback();
    }*/
    
    private void DeadByExecution()
    {
        if (isDead) return;
        currentHp = 0;
        isDead = true;
        enemy.SetEnemyDead();
        vfx.DeathVFX_Feedback();
    }
}
