using UnityEngine;

public class Enemy_FlinchState : EnemyState
{
    public Enemy_FlinchState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        /*enemy.agent.isStopped = true;
        stateTimer = enemy.flinchTime;*/
        enemy.Flinch_Enter();
    }

    public override void Update()
    {
        base.Update();
        /*GetDistanceToPlayer();
        if (enemy.health.currentHp <= 0)
        {
            enemy.DeadEntity();
            return;
        }
        stateMachine.ChangeState(enemy.moveState);*/
        enemy.Flinch_Update();
    }
    public override void Exit()
    {
        base.Exit();
        //enemy.agent.isStopped = false;
        enemy.Flinch_Exit();
    }

}
