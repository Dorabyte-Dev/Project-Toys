using UnityEngine;

public class Enemy_FlinchState : EnemyState
{
    public Enemy_FlinchState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        enemy.agent.isStopped = true;
        stateTimer = enemy.flinchTime;
    }

    public override void Update()
    {
        base.Update();
        // if(stateTimer <= 0)
        // {
        //     switch (enemy.nearness)
        //     {
        //         case 1:
        //             stateMachine.ChangeState(enemy.pursuitState);
        //             break;
        //         case 2:
        //             stateMachine.ChangeState(enemy.waitAttackState);
        //             break;
        //         case 0:
        //             stateMachine.ChangeState(enemy.idleState);
        //             break;
        //     }
        // }
        GetDistanceToPlayer();
        if (enemy.health.currentHp <= 0)
        {
            enemy.DeadEntity();
            return;
        }
        stateMachine.ChangeState(enemy.moveState);
    }
    public override void Exit()
    {
        base.Exit();
        enemy.agent.isStopped = false;
    }

}
