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

    public void DisableAndDestroyEnemy()
    {
        //Destroy(gameObject);
        Debug.Log("[Enemy_AnimationTriggers] DisableAndDestroyEnemy called");
        enemy.agent.isStopped = true;
        enemy.mesh.enabled = false;     //De momento se queda así hasta que se aplique bien el efecto de dissolve, para que no se quede el modelo en medio sin hacer nada.
        enemy._health.enemyUI.canvas.SetActive(false);
        Invoke(nameof(DestroyEnemy), 5);        //De momento se queda así hasta que se aplique la pool de los enemigos.
    }
    
    private void DestroyEnemy()
    {
        Destroy(gameObject);
    }
    
    public void AttackFinished()
    {
        enemy.ChangeEnemyState(enemy.pursuitState);
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
