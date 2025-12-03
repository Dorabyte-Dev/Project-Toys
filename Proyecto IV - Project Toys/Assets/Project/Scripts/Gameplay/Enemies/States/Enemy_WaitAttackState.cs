using UnityEngine;

public class Enemy_WaitAttackState : EnemyState
{
    private float currentTime;

    public Enemy_WaitAttackState(Enemy enemy, StateMachine stateMachine, string animBoolName)
        : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        Debug.Log($"[{enemy.name}] Entra en WaitAttackState.");

        // Notificar al manager que este enemigo está atacando
        enemy.NotifyAttackStarted();

        currentTime = 0f;

        // Detener al enemigo
        enemy.agent.isStopped = true;
        enemy.agent.velocity = Vector3.zero;

        // Mirar al jugador
        if (enemy.playerTransform != null)
        {
            Vector3 direction = enemy.playerTransform.position - enemy.transform.position;
            direction.y = 0;
            if (direction != Vector3.zero)
            {
                enemy.transform.rotation = Quaternion.LookRotation(direction);
            }
        }
    }

    public override void Update()
    {
        base.Update();

        // Solo abortar si se pierde completamente al jugador
        // if (enemy.nearness <= 0 || enemy.playerTransform == null)
        // {
        //     Debug.Log($"[{enemy.name}] Pierde COMPLETAMENTE al jugador durante preparación -> Pursuit");
        //     stateMachine.ChangeState(enemy.pursuitState);
        //     return;
        // }

        currentTime += Time.deltaTime;

        if (currentTime >= enemy.waitTime)
        {
            Debug.Log($"[{enemy.name}] Preparación lista -> AttackState");

            Vector3 attackPoint = CalculateAttackPoint();
            enemy.attackState.SetParametersAttack(enemy.transform.position, attackPoint);

            enemy.agent.isStopped = false;
            stateMachine.ChangeState(enemy.attackState);
        }
    }

    private Vector3 CalculateAttackPoint()
    {

        Vector3 enemyPos = enemy.transform.position;
        Vector3 playerPos = enemy.playerTransform.position;

        Vector3 dir = (playerPos - enemyPos);
        dir.y = 0;
        
        if (dir.sqrMagnitude < 0.01f)
        {
            // Si está muy encima del player, usar forward del enemigo
            dir = enemy.transform.forward;
        }
        else
        {
            dir.Normalize();
        }

        float dashDistance = 5f; // Distancia de embestida
        Vector3 attackPoint = playerPos + dir * dashDistance;

        Debug.Log($"[{enemy.name}] AttackPoint calculado: {attackPoint} (player en {playerPos})");
        return attackPoint;
    }

    public override void Exit()
    {
        base.Exit();
        enemy.agent.isStopped = false;
    }
}