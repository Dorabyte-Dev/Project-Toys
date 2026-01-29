using UnityEngine;

public class Enemy_IdleState : EnemyState
{
    public Enemy_IdleState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        //stateTimer = 0.25f;
        //enemy.SetVelocity(0f, 0f);
        enemy.Idle_Enter();
    }

    public override void Update()
    {
        base.Update();
        /*if (stateTimer < 0f)
            stateMachine.ChangeState(enemy.moveState);*/
        enemy.Idle_Update();
    }
    public override void Exit()
    {
        base.Exit();
        enemy.Idle_Exit();
    }
}