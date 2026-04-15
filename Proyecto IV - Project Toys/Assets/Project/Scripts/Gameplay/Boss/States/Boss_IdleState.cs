using UnityEngine;

public class Boss_IdleState : BossState
{
    public Boss_IdleState(Boss boss, StateMachine stateMachine, string animBoolName) : base(boss, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        base.Update();
        if (boss.player)
        {
            boss.ChangeBossState(boss.attackState);
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}
