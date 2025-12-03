using UnityEngine;

public class Enemy_AttackState : EnemyState
{
    private Vector3 attackPoint;
    private Vector3 currentPosition;
    private float attackTimer;
    private bool hasStartedDash;
    
    private const float maxAttackDuration = 2f;

    public Enemy_AttackState(Enemy enemy, StateMachine stateMachine, string animBoolName)
        : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        attackTimer = 0f;
        hasStartedDash = false;

        // Marcar que está atacando
        enemy.isAttacking = true;

        Debug.Log($"[{enemy.name}] ENTER AttackState -> attackPoint={attackPoint}");

        // Detener al enemigo inicialmente (preparación)
        enemy.agent.isStopped = true;
        enemy.agent.velocity = Vector3.zero;

        // Mirar hacia el punto de ataque
        Vector3 lookDirection = attackPoint - enemy.transform.position;
        lookDirection.y = 0;
        if (lookDirection != Vector3.zero)
        {
            enemy.transform.rotation = Quaternion.LookRotation(lookDirection);
        }
    }

    public override void Update()
    {
        base.Update();

        attackTimer += Time.deltaTime;

        // Fase 1: Esperar un momento antes de lanzar el dash (preparación visual)
        if (!hasStartedDash)
        {
            StartDash();
        }

        // Fase 2: Durante el dash
        if (hasStartedDash)
        {
            // Dibuja la línea para depuración
            Debug.DrawLine(enemy.transform.position, attackPoint, Color.red);

            float dist = Vector3.Distance(enemy.transform.position, attackPoint);

            // Log cada 10 frames
            if (Time.frameCount % 10 == 0)
            {
                Debug.Log($"[Attack {enemy.name}] t={attackTimer:F2}s, dist={dist:F2}, vel={enemy.agent.velocity.magnitude:F2}");
            }

            // Condición de salida 1: llegó al punto
            if (HasReachedDestination())
            {
                Debug.Log($"[{enemy.name}] Attack reached target point (dist={dist:F2}).");
                FinishAttack();
                return;
            }

            // Condición de salida 2: timeout
            if (attackTimer >= maxAttackDuration)
            {
                Debug.Log($"[{enemy.name}] Attack timeout (t={attackTimer:F2}).");
                FinishAttack();
                return;
            }
        }
    }

    private void StartDash()
    {
        hasStartedDash = true;

        Debug.Log($"[{enemy.name}] Inicia DASH hacia {attackPoint}");

        // Configurar velocidad de ataque
        enemy.agent.isStopped = false;
        enemy.agent.speed = enemy.attackSpeed;
        enemy.agent.acceleration = enemy.attackAcceleration;

        // Lanzar el dash
        enemy.agent.SetDestination(attackPoint);

        // Trigger animación de ataque (si tienes)
        // anim.SetTrigger("Attack");
    }

    private bool HasReachedDestination()
    {
        if (!enemy.agent.pathPending && enemy.agent.isOnNavMesh)
        {
            float arrivalDestinationThreshold = enemy.agent.stoppingDistance + 0.5f;
            
            if (enemy.agent.remainingDistance <= arrivalDestinationThreshold)
            {
                // Verificar también que la velocidad sea baja (casi parado)
                if (!enemy.agent.hasPath || enemy.agent.velocity.sqrMagnitude < 0.1f)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public override void Exit()
    {
        base.Exit();
        Debug.Log($"[{enemy.name}] EXIT AttackState.");
        
        // Notificar fin de ataque (esto pone isAttacking = false y inicia cooldown)
        enemy.NotifyAttackFinished();

        // Restaurar velocidad normal
        enemy.agent.speed = enemy.moveSpeed;
        enemy.agent.acceleration = enemy.acceleration;
        enemy.agent.isStopped = false;
    }

    private void FinishAttack()
    {
        stateMachine.ChangeState(enemy.pursuitState);
    }

    public void SetParametersAttack(Vector3 currentP, Vector3 attackP)
    {
        currentPosition = currentP;
        attackPoint = attackP;
        Debug.Log($"[{enemy.name}] SetParametersAttack: current={currentP}, target={attackP}");
    }
}