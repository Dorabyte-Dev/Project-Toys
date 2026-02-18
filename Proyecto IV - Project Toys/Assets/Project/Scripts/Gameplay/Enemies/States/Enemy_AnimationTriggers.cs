using UnityEngine;

public class Enemy_AnimationTriggers : Entity_AnimationTriggers
{
    [SerializeField]private Enemy enemy;

    public override void Awake()
    {
        base.Awake();
        enemy = GetComponent<Enemy>();
    }

    public void NotifyAttackFinished()
    {
        enemy.isAttacking = false;
    }

    public void DestroyEnemy()
    {
        Destroy(gameObject);
        /*enemy.mesh.enabled = false;
        Invoke(nameof(Destroy), 5);*/
    }
    
    public void DeathAnimationTrigger()
    {
        enemy.ChangeEnemyState(enemy.deadState);
    }
    
    public void FinishInvokeProjectile()
    {
        enemy.ChangeEnemyState(enemy.idleState);
    }

    public override void AttackTrigger()
    {
        if(enemy.hasAttacked) return;
        base.AttackTrigger();
    }

    public override void HeavyTrigger()
    {
        if(enemy.hasAttacked) return;
        base.HeavyTrigger();
    }
}
