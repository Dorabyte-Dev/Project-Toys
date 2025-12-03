using UnityEngine;

public class Enemy_PursuitState : EnemyState
{
    private bool hasRequestedAttack = false;

    public Enemy_PursuitState(Enemy enemy, StateMachine stateMachine, string animBoolName)
        : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        Debug.Log($"[{enemy.name}] Entra en PursuitState.");

        enemy.agent.isStopped = false;
        enemy.agent.speed = enemy.moveSpeed;
        enemy.agent.acceleration = enemy.acceleration;
        
        hasRequestedAttack = false; // Reset al entrar
    }

    public override void Update()
    {
        base.Update();

        if (enemy.playerTransform == null)
            return;

        if (enemy.nearness == 2)
        {
            // En rango de ataque
            HandleAttackRangeLogic();
        }
        else if (enemy.nearness == 1)
        {
            // Perseguir normalmente
            if (hasRequestedAttack)
            {
                // Si salió de rango de ataque, cancelar solicitud
                EnemyWaveManager.Instance.CancelAttackRequest(enemy);
                hasRequestedAttack = false;
                Debug.Log($"[{enemy.name}] Salió de rango, cancela solicitud.");
            }
            enemy.agent.SetDestination(enemy.playerTransform.position);
        }
        else
        {
            // nearness == 0, muy lejos
            if (hasRequestedAttack)
            {
                EnemyWaveManager.Instance.CancelAttackRequest(enemy);
                hasRequestedAttack = false;
            }
            stateMachine.ChangeState(enemy.idleState);
        }
    }

    public override void Exit()
    {
        base.Exit();
        
        // Si sale del estado sin haber atacado, cancelar solicitud
        if (hasRequestedAttack && !enemy.isAttacking)
        {
            EnemyWaveManager.Instance.CancelAttackRequest(enemy);
            hasRequestedAttack = false;
            Debug.Log($"[{enemy.name}] Sale de PursuitState, cancela solicitud.");
        }
    }

    private void HandleAttackRangeLogic()
    {
        // Si está en cooldown, solo orbitar (no puede pedir permiso)
        if (enemy.isOnAttackCooldown)
        {
            Debug.Log($"[{enemy.name}] En cooldown, orbitando...");
            OrbitAroundPlayer();
            return;
        }

        // Si ya está atacando, no hacer nada
        if (enemy.isAttacking)
            return;

        // Si ya tiene permiso del manager, pasar a WaitAttack
        if (enemy.canAttackByManager)
        {
            Debug.Log($"[{enemy.name}] Tiene permiso del WaveManager -> WaitAttackState");
            stateMachine.ChangeState(enemy.waitAttackState);
            return;
        }

        // Si NO tiene permiso y NO lo ha pedido aún, pedirlo
        if (!hasRequestedAttack)
        {
            Debug.Log($"[{enemy.name}] Pide permiso al WaveManager para atacar.");
            EnemyWaveManager.Instance.RequestAttackPermission(enemy);
            hasRequestedAttack = true;
        }

        // Mientras espera permiso, orbitar
        OrbitAroundPlayer();
    }

    private void OrbitAroundPlayer()
    {
        if (enemy.playerTransform == null) return;

        enemy.orbitAngle += Time.deltaTime * enemy.orbitSpeed;
        if (enemy.orbitAngle > 360f)
            enemy.orbitAngle -= 360f;

        float angleRad = enemy.orbitAngle * Mathf.Deg2Rad;

        Vector3 targetPosition;
        targetPosition.x = enemy.playerTransform.position.x + enemy.orbitDistance * Mathf.Cos(angleRad);
        targetPosition.y = enemy.transform.position.y;
        targetPosition.z = enemy.playerTransform.position.z + enemy.orbitDistance * Mathf.Sin(angleRad);

        enemy.agent.SetDestination(targetPosition);

        Vector3 lookDirection = enemy.playerTransform.position - enemy.transform.position;
        lookDirection.y = 0;
        if (lookDirection != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(lookDirection);
            enemy.transform.rotation = Quaternion.Slerp(enemy.transform.rotation, targetRot, Time.deltaTime * 5f);
        }
    }
}