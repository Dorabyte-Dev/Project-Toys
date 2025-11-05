using UnityEngine;

public class Enemy_AttackState : EnemyState
{
    private Vector3 attackPoint;
    private Vector3 lastPlayerPosition;
    private Vector3 currentPosition;
    private float currentTime;
    //private bool hasStartedAttack;

    public Enemy_AttackState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

    }

    public override void Update()
    {
        base.Update();

        //hasStartedAttack = true;

        Vector3 attackDirection = (attackPoint - currentPosition).normalized;
        attackPoint = currentPosition + attackDirection * enemy.attackRange;

        //anim.Play("Attack");

        enemy.agent.speed = enemy.attackSpeed;
        enemy.agent.acceleration = enemy.attackAcceleration;
        enemy.agent.destination = attackPoint;

        // Verificar si llegó al punto de ataque
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
        enemy.damageCollider.SetActive(false);

        // Restaurar velocidades normales
        enemy.agent.speed = enemy.moveSpeed;
        enemy.agent.acceleration = enemy.acceleration;
    }

    private bool HasReachedDestination()
    {
        if (!enemy.agent.pathPending)
        {
            if (enemy.agent.remainingDistance <= enemy.agent.stoppingDistance)
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
        if (!enemy.isAttacking)
        {
            stateMachine.ChangeState(enemy.idleState);
        }
        else
        {
            stateMachine.ChangeState(enemy.pursuitState);
        }
    }

    public void SetParametersAttack(Vector3 currentP, Vector3 attackP) 
    {
        currentPosition = currentP;
        attackPoint = attackP;
    }
}