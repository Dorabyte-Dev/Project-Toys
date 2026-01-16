using System.Collections;
using UnityEngine;

public class Enemy_PursuitState : EnemyState
{
    //private bool hasRequestedAttack;

    public Enemy_PursuitState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        /*enemy.agent.speed = enemy.moveSpeed;
        enemy.agent.acceleration = enemy.acceleration;
        //hasRequestedAttack = false;
        //Debug.Log("Entering Pursuit State");*/
        enemy.Pursuit_Enter();
    }

    public override void Update()
    {
        base.Update();
        /*GetDistanceToPlayer();
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
        }*/
        enemy.Pursuit_Update();
    }

    public override void Exit()
    {
        base.Exit();
        enemy.Pursuit_Exit();
    }
    
    
}