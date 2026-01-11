using UnityEngine;

public class Enemy_AttackState : EnemyState
{
    private Vector3 attackPoint;
    private Vector3 lastPlayerPosition;
    private Vector3 currentPosition;
    private float currentTime;
    private bool reachedAttack;

    public Enemy_AttackState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        
        //enemy.isAttacking = true;
        //Debug.Log($"[Enemy_AttackState] {enemy.name} entra en ataque.");

        // Configurar velocidad de ataque (embestida rápida)
        enemy.agent.speed = enemy.attackSpeed;
        enemy.agent.acceleration = enemy.attackAcceleration;
        enemy.agent.destination = attackPoint;
    }

    public override void Update()
    {
        base.Update();

        // Verificar si llegó al punto de ataque
        if (HasReachedDestination())
        {
            enemy.StopAttacking();
        }
    }

    public override void Exit()
    {
        base.Exit();
        
        //Debug.Log($"[Enemy_AttackState] {enemy.name} sale del ataque.");
        
        // IMPORTANTE: Notificar al manager que terminó el ataque
        //enemy.NotifyAttackFinished();
        EnemyWaveManager.Instance.NotifyEnemyFinishedAttack(enemy);
        
        reachedAttack = false;
        enemy.hasAttacked = false;
        
        // Restaurar velocidades normales
        enemy.agent.speed = enemy.moveSpeed;
        enemy.agent.acceleration = enemy.acceleration;
    }

    private bool HasReachedDestination()
    {
        if (!enemy.agent.pathPending)
        {
            float arrivalDestinationThreshold = enemy.agent.stoppingDistance + 0.5f;
            if (enemy.agent.remainingDistance <= arrivalDestinationThreshold)
            {
                if (!enemy.agent.hasPath || enemy.agent.velocity.sqrMagnitude == 0f)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void FinishAttack()
    {
        // Cambiar al estado de persecución
        stateMachine.ChangeState(enemy.pursuitState);
    }

    public void SetParametersAttack(Vector3 currentP, Vector3 attackP)
    {
        currentPosition = currentP;
        attackPoint = attackP;
        reachedAttack = true;
    }
}