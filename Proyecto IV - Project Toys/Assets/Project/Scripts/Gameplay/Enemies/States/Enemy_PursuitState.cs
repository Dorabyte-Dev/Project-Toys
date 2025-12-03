using System.Collections;
using UnityEngine;

public class Enemy_PursuitState : EnemyState
{
    private bool hasRequestedAttack;

    public Enemy_PursuitState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        enemy.agent.speed = enemy.moveSpeed;
        enemy.agent.acceleration = enemy.acceleration;
        hasRequestedAttack = false;
    }

    public override void Update()
    {
        base.Update();
        GetDistanceToPlayer();
        // Perseguir al jugador
        if (enemy.playerTransform != null)
        {
            enemy.agent.destination = enemy.playerTransform.position;
            if (enemy.distanceToPlayer < enemy.attackPlayerRange)
            {
                stateMachine.ChangeState(enemy.waitAttackState);
            }
            else if (enemy.distanceToPlayer > enemy.pursuitPlayerRange)
            {
                stateMachine.ChangeState(enemy.moveState);
            }
        }

        // switch (enemy.nearness)
        // {
        //     case 2: // En rango de ataque
        //         // Solicitar permiso si aún no lo ha hecho
        //         if (!hasRequestedAttack && !enemy.isAttacking)
        //         {
        //             EnemyWaveManager.Instance?.RequestAttackPermission(enemy);
        //             hasRequestedAttack = true;
        //         }
        //
        //         // Si tiene permiso, atacar
        //         if (enemy.canAttackByManager && !enemy.isAttacking)
        //         {
        //             stateMachine.ChangeState(enemy.waitAttackState);
        //         }
        //         break;
        //
        //     case 1: // Sigue persiguiendo
        //         // Si había solicitado ataque pero salió de rango, cancelar
        //         if (hasRequestedAttack)
        //         {
        //             EnemyWaveManager.Instance?.CancelAttackRequest(enemy);
        //             hasRequestedAttack = false;
        //         }
        //         break;
        //
        //     case 0: // Muy lejos, volver a idle
        //         if (hasRequestedAttack)
        //         {
        //             EnemyWaveManager.Instance?.CancelAttackRequest(enemy);
        //             hasRequestedAttack = false;
        //         }
        //         stateMachine.ChangeState(enemy.idleState);
        //         break;
        // }
    }

    public override void Exit()
    {
        base.Exit();
        
        // Si sale del estado sin haber atacado, cancelar solicitud
        if (hasRequestedAttack && !enemy.isAttacking)
        {
            EnemyWaveManager.Instance?.CancelAttackRequest(enemy);
            hasRequestedAttack = false;
        }
    }
    
    
}