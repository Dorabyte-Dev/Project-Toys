using UnityEngine;

public class Enemy_AttackState : EnemyState
{
    private Vector3 attackPoint;
    private Vector3 lastPlayerPosition;
    private Vector3 currentPosition;
    private float currentTime;
    private bool hasStartedAttack;

    public Enemy_AttackState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        enemy.isAttacking = true;
        enemy.damageCollider.SetActive(true);

        // Guardar posiciones
        currentPosition = enemy.transform.position;
        lastPlayerPosition = enemy.playerTransform != null ? enemy.playerTransform.position : enemy.transform.forward;

        // Calcular punto de ataque
        enemy.agent.destination = currentPosition + (lastPlayerPosition - currentPosition) * 0.25f;
        attackPoint = lastPlayerPosition;

        // Mirar hacia el objetivo
        enemy.transform.LookAt(new Vector3(attackPoint.x, enemy.transform.position.y, attackPoint.z), Vector3.up);

        // Animación de carga
        anim.Play("WaitAttack");

        currentTime = 0f;
        hasStartedAttack = false;
    }

    public override void Update()
    {
        base.Update();

        currentTime += Time.deltaTime;

        if (currentTime >= enemy.waitTime && !hasStartedAttack)
        {
            // Ejecutar el ataque
            hasStartedAttack = true;

            Vector3 attackDirection = (attackPoint - currentPosition).normalized;
            attackPoint = currentPosition + attackDirection * enemy.attackRange;

            anim.Play("Attack");

            enemy.agent.speed = enemy.attackSpeed;
            enemy.agent.acceleration = enemy.attackAcceleration;
            enemy.agent.destination = attackPoint;
        }

        // Verificar si llegó al punto de ataque
        if (hasStartedAttack && HasReachedDestination())
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
        stateMachine.ChangeState(enemy.pursuitState);
    }
}