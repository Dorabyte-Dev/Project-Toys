using UnityEngine;

public class Enemy_AttackState : EnemyState
{
    private Vector3 attackPoint;
    private Vector3 lastPlayerPosition;
    private Vector3 currentPosition;
    private float currentTime;
    private bool reachedAttack;
    //private bool hasStartedAttack;

    public Enemy_AttackState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        
        enemy.isAttacking = true;
        //enemy.damageCollider.SetActive(true);

        // Configurar velocidad de ataque (embestida r�pida)
        enemy.agent.speed = enemy.attackSpeed;
        enemy.agent.acceleration = enemy.attackAcceleration;
        enemy.agent.destination = attackPoint;

    }

    public override void Update()
    {
        base.Update();

        // Verificar si lleg� al punto de ataque
        //if (hasStartedAttack && HasReachedDestination())
        if (HasReachedDestination())
        {
            FinishAttack();
        }
    }

    public override void Exit()
    {
        base.Exit();
        enemy.isAttacking = false;
        //enemy.damageCollider.SetActive(false);
        reachedAttack = false;

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
        // Cambiar al estado de persecuci�n
        stateMachine.ChangeState(enemy.pursuitState);
    }

    public void SetParametersAttack(Vector3 currentP, Vector3 attackP)
    {
        currentPosition = currentP;
        attackPoint = attackP;
        reachedAttack = true;
    }
    
    
}