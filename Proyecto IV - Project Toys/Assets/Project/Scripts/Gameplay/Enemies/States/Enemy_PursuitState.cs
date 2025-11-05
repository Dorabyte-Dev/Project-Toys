using UnityEngine;

public class Enemy_PursuitState : EnemyState
{
    public Enemy_PursuitState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        enemy.agent.speed = enemy.moveSpeed;
        enemy.agent.acceleration = enemy.acceleration;
    }

    public override void Update()
    {
        base.Update();

        // Perseguir al jugador
        if (enemy.playerTransform != null)
        {
            enemy.agent.destination = enemy.playerTransform.position;
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}