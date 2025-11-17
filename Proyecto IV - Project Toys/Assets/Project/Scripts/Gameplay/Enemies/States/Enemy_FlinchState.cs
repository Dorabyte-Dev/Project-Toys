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
        if(stateTimer <= 0)
        {
            stateMachine.ChangeState(enemy.idleState);
        }
    }
    public override void Exit()
    {
        base.Exit();
        enemy.agent.isStopped = false;
    }

}
